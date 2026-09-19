using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;
using Paramore.Brighter;
using System.Text;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class PublishBookRequest(int libraryId, int bookId) : LibraryBaseCommand(libraryId)
{
    public string OutputType { get; set; }
    public string Result { get; set; }
    public int BookId { get; } = bookId;
    public bool OnlyPublishFile { get; set; }
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
        var chapterTexts = new Dictionary<string, string>();

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
                
                chapterTexts.Add(chapter.Title, finalContent ?? string.Empty);
            }
            else
            {
                var pages = await bookPageRepository.GetPagesByBookChapter(command.LibraryId, command.BookId,
                    chapter.Id, cancellationToken);
                var finalText = await CombinePages(pages, cancellationToken);
                chapterTexts.Add(chapter.Title, finalText);

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
                    ;
                    await commandProcessor.SendAsync(cmd, cancellationToken: cancellationToken);
                }
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
                        chapterTexts.TryGetValue(x.Title, out var text) ? text : string.Empty
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
            IsPublic = false,
            Checksum = ChecksumHelper.Compute(contents)
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
        existingDocx.Checksum = ChecksumHelper.Compute(file);

        await fileRepository.UpdateFile(existingDocx, cancellationToken);
    }

    private char[] pageBreakSymbols = new char[] { '۔', ':', '“', '"', '\'', '!' };

    private async Task<string> CombinePages(IEnumerable<BookPageModel> pages, CancellationToken cancellationToken)
    {
        StringBuilder builder = new StringBuilder();

        var tasks = pages.Select(GetPageText).ToArray();
        
        await Task.WhenAll(tasks); 
        
        foreach (var task in tasks)
        {
            var (separator, finalText) = task.Result;

            builder.Append(separator);
            builder.Append(finalText);
        }

        return builder.ToString().TrimStart();

        async Task<(string separator, string finalText)> GetPageText(BookPageModel page)
        {
            var separator = " ";
            if (page.ContentId.HasValue)
            {
                var file = await fileRepository.GetFileById(page.ContentId.Value, cancellationToken);
                if (file != null)
                {
                    page.Text = await fileStorage.GetTextFile(file.FilePath, cancellationToken);
                }
            }
            var finalText = page.Text.Trim();
            if (string.IsNullOrWhiteSpace(finalText))
            {
                return (separator, finalText);
            }

            var lastCharacter = finalText.Last();

            if (pageBreakSymbols.Contains(lastCharacter))
            {
                separator = Environment.NewLine;
            }

            return (separator, finalText);
        }
    }
}
