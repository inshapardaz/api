using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class PublishBookRequest(int libraryId, int bookId) : LibraryBaseCommand(libraryId)
{
    public string OutputType { get; set; }
    public string Result { get; set; }
    public int BookId { get; } = bookId;
    public bool OnlyPublishFile { get; set; }

    // When combining page text produces a chapter that already has different content,
    // the handler refuses to overwrite it (via ConflictException) unless the caller has
    // asked the user and resubmitted the request with this set to true.
    public bool ConfirmOverwrite { get; set; }

    // Chapters whose existing content was kept rather than replaced, because the pages
    // combined to blank text -- surfaced back to the caller as a non-fatal warning.
    public List<string> Warnings { get; } = new List<string>();
}

public class PublishBookRequestHandler(
    IBookRepository bookRepository,
    IChapterRepository chapterRepository,
    IBookPageRepository bookPageRepository,
    IWriteWordDocument wordDocumentWriter,
    IFileStorage fileStorage,
    IFileRepository fileRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<PublishBookRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin)]
    public override async Task<PublishBookRequest> HandleAsync(PublishBookRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
        byte[] bookImage = null;
        if (book.ImageId.HasValue)
        {
            var file = await fileRepository.GetFileById(book.ImageId.Value, cancellationToken);
            if (file != null)
            {
                bookImage = await fileStorage.GetFile(file.FilePath, cancellationToken);
            }
        }
        var chapters = await chapterRepository.GetChaptersByBook(command.LibraryId, command.BookId, cancellationToken);
        // Keyed by ChapterNumber, not Title -- chapter titles aren't unique within a book, and
        // Dictionary<string, string>.Add on a duplicate title used to throw, making it
        // impossible to publish any book with two same-named chapters.
        var chapterTexts = new Dictionary<int, string>();
        // ChapterNumber -> combined page text, for chapters whose content actually needs
        // writing. Resolved up front so a conflicting chapter can abort the whole publish
        // before anything is persisted.
        var chaptersToPersist = new Dictionary<int, string>();
        var conflictingChapters = new List<string>();

        foreach (var chapter in chapters)
        {
            if (command.OnlyPublishFile)
            {
                string finalContent = string.Empty;
                var chapterContent = await chapterRepository.GetChapterContent(command.LibraryId, book.Id, chapter.ChapterNumber, book.Language,
                    cancellationToken);
                if (chapterContent != null && chapterContent.FileId.HasValue)
                {
                    var file = await fileRepository.GetFileById(chapterContent.FileId.Value, cancellationToken);
                    finalContent = await fileStorage.GetTextFile(file.FilePath, cancellationToken);
                }

                chapterTexts.Add(chapter.ChapterNumber, finalContent ?? string.Empty);
            }
            else
            {
                var pages = await bookPageRepository.GetPagesByBookChapter(command.LibraryId, command.BookId,
                    chapter.Id, cancellationToken);
                var combinedText = await CombinePages(pages, cancellationToken);

                var existingContent = await chapterRepository.GetChapterContent(command.LibraryId, book.Id, chapter.ChapterNumber, book.Language,
                    cancellationToken);
                var existingText = await GetExistingChapterText(existingContent, cancellationToken);

                if (string.IsNullOrWhiteSpace(combinedText) && !string.IsNullOrWhiteSpace(existingText))
                {
                    // The pages have no text yet -- never wipe out chapter content that's
                    // already there, just warn and keep what's there.
                    chapterTexts.Add(chapter.ChapterNumber, existingText);
                    command.Warnings.Add(
                        $"Chapter '{chapter.Title}' has no text in its pages; its existing content was kept.");
                    continue;
                }

                var textChanged = existingText == null || existingText.Trim() != combinedText.Trim();

                if (textChanged && !string.IsNullOrWhiteSpace(existingText) && !command.ConfirmOverwrite)
                {
                    conflictingChapters.Add(chapter.Title);
                }

                chapterTexts.Add(chapter.ChapterNumber, combinedText);

                if (textChanged)
                {
                    chaptersToPersist.Add(chapter.ChapterNumber, combinedText);
                }
            }
        }

        if (conflictingChapters.Count > 0)
        {
            throw new ConflictException(
                $"The following chapters already have content that would be overwritten by their page text: {string.Join(", ", conflictingChapters)}. Publish again confirming the overwrite to proceed.");
        }

        foreach (var chapter in chapters)
        {
            if (!chaptersToPersist.TryGetValue(chapter.ChapterNumber, out var finalText))
            {
                continue;
            }

            if (chapter.Contents.Any(cc => cc.Language == book.Language))
            {
                var cmd = new UpdateChapterContentRequest(command.LibraryId, command.BookId, chapter.ChapterNumber,
                    finalText, book.Language);
                await commandProcessor.SendAsync(cmd, cancellationToken: cancellationToken);
            }
            else
            {
                var cmd = new AddChapterContentRequest(command.LibraryId, command.BookId, chapter.ChapterNumber,
                    finalText, book.Language);
                await commandProcessor.SendAsync(cmd, cancellationToken: cancellationToken);
            }
        }

        byte[] outputFile = null;
        if (command.OutputType == MimeTypes.MsWord)
        {
            outputFile = wordDocumentWriter.ConvertMarkdownToWord(chapterTexts.Values);
        }
        else if (command.OutputType == MimeTypes.Epub)
        {
            var converter = new MarkdownToEpubConverter();
            outputFile = converter.CreateEpub(
                book, 
                chapters.Select(x => 
                    new MarkdownToEpubConverter.Chapter(
                        x.Title,
                        chapterTexts.TryGetValue(x.ChapterNumber, out var text) ? text : string.Empty
                    )).ToList(), 
                $"{book.Title.ToSafeFilename()}.epub",
                bookImage
            );
        }
        else
        {
            return await base.HandleAsync(command, cancellationToken);
        }

        var bookContent = await bookRepository.GetBookContent(command.LibraryId, command.BookId, book.Language, command.OutputType, cancellationToken);

        if (bookContent == null)
        {
            FileModel file = await SaveFileToStorage(book, outputFile, command.OutputType, cancellationToken);
            await bookRepository.AddBookContent(command.BookId, file.Id, book.Language, cancellationToken);
        }
        else
        {
            await UpdateFileInStorage(book, bookContent.FileId, outputFile, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }

    private async Task<FileModel> SaveFileToStorage(BookModel book, byte[] contents, string mimeType, CancellationToken cancellationToken)
    {
        string fileName = null;
        switch (mimeType)
        {
            case MimeTypes.MsWord:
                fileName = $"{book.Title.ToSafeFilename()}.docx";
                break;
            case MimeTypes.Epub:
                fileName = $"{book.Title.ToSafeFilename()}.epub";
                break;
            default:
                throw new NotSupportedException($"Mime type '{mimeType}' is not supported.");
            
        }
        var url = await fileStorage.StoreFile($"books/{book.Id}/{fileName}", contents, mimeType, cancellationToken);
        var file = await fileRepository.AddFile(new FileModel
        {
            FilePath = url,
            MimeType = mimeType,
            FileName = fileName,
            IsPublic = false
        }, cancellationToken);
        return file;
    }

    private async Task UpdateFileInStorage(BookModel book, long fileId, byte[] file, CancellationToken cancellationToken)
    {
        var fileName = $"{book.Title.ToSafeFilename()}.docx";
        var existingDocx = await fileRepository.GetFileById(fileId, cancellationToken);
        if (existingDocx != null && !string.IsNullOrWhiteSpace(existingDocx.FilePath))
        {
            await fileStorage.DeleteFile(existingDocx.FilePath, cancellationToken);
        }

        existingDocx.FilePath = await fileStorage.StoreFile($"books/{book.Id}/{fileName}", file, MimeTypes.MsWord, cancellationToken);

        await fileRepository.UpdateFile(existingDocx, cancellationToken);
    }

    // Likhari's markdown representation of a page-break block (see
    // @inshapardaz/likhari-converters markdown/serialize.js, the 'page-break' case) --
    // marks where one scanned page ends and the next begins, instead of blurring the
    // boundary away with a plain space or newline.
    private const string PageBreakMarker = "\n\n::pagebreak\n\n";

    private async Task<string> CombinePages(IEnumerable<BookPageModel> pages, CancellationToken cancellationToken)
    {
        var texts = await Task.WhenAll(pages.Select(GetPageText));

        var nonEmptyTexts = texts.Where(text => !string.IsNullOrWhiteSpace(text));

        return string.Join(PageBreakMarker, nonEmptyTexts).Trim();

        async Task<string> GetPageText(BookPageModel page)
        {
            if (page.ContentId.HasValue)
            {
                var file = await fileRepository.GetFileById(page.ContentId.Value, cancellationToken);
                if (file != null)
                {
                    page.Text = await fileStorage.GetTextFile(file.FilePath, cancellationToken);
                }
            }

            return page.Text?.Trim() ?? string.Empty;
        }
    }

    private async Task<string> GetExistingChapterText(ChapterContentModel content, CancellationToken cancellationToken)
    {
        if (content == null)
        {
            return null;
        }

        if (content.FileId.HasValue)
        {
            var file = await fileRepository.GetFileById(content.FileId.Value, cancellationToken);
            if (file != null)
            {
                return await fileStorage.GetTextFile(file.FilePath, cancellationToken);
            }
        }

        return content.Text;
    }
}
