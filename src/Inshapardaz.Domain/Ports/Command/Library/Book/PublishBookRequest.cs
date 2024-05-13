using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Book
{
    public class PublishBookRequest : RequestBase
    {
        public PublishBookRequest(int libraryId, int bookId)
        {
            LibraryId = libraryId;
            BookId = bookId;
        }

        public string Result { get; set; }
        public int LibraryId { get; }
        public int BookId { get; }
    }

    public class PublishBookRequestHandler : RequestHandlerAsync<PublishBookRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IBookPageRepository _bookPageRepository;
        private readonly IWriteWordDocument _wordDocumentWriter;
        private readonly IFileStorage _fileStorage;
        private readonly IFileRepository _fileRepository;

        public PublishBookRequestHandler(IBookRepository bookRepository,
            IChapterRepository chapterRepository,
            IBookPageRepository bookPageRepository,
            IWriteWordDocument wordDocumentWriter,
            IFileStorage fileStorage,
            IFileRepository fileRepository)
        {
            _bookRepository = bookRepository;
            _chapterRepository = chapterRepository;
            _bookPageRepository = bookPageRepository;
            _wordDocumentWriter = wordDocumentWriter;
            _fileStorage = fileStorage;
            _fileRepository = fileRepository;
        }

        [LibraryAuthorize(1, Role.LibraryAdmin)]
        public override async Task<PublishBookRequest> HandleAsync(PublishBookRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
            var chapters = await _chapterRepository.GetChaptersByBook(command.LibraryId, command.BookId, cancellationToken);
            var chapterText = new List<string>();
            foreach (var chapter in chapters)
            {
                var pages = await _bookPageRepository.GetPagesByBookChapter(command.LibraryId, command.BookId, chapter.Id, cancellationToken);
                var finalText = CombinePages(pages);
                chapterText.Add(finalText);
                if (chapter.Contents.Any(cc => cc.Language == book.Language))
                {
                    await _chapterRepository.UpdateChapterContent(command.LibraryId, command.BookId, chapter.ChapterNumber, book.Language, finalText, cancellationToken);
                }
                else
                {
                    await _chapterRepository.AddChapterContent(command.LibraryId, new ChapterContentModel
                    {
                        BookId = command.BookId,
                        ChapterId = chapter.Id,
                        Language = book.Language,
                        ChapterNumber = chapter.ChapterNumber,
                        Text = finalText
                    }, cancellationToken);
                }
            }

            var wordDocument = _wordDocumentWriter.ConvertMarkdownToWord(chapterText);

            var bookContent = await _bookRepository.GetBookContent(command.LibraryId, command.BookId, book.Language, MimeTypes.MsWord, cancellationToken);

            if (bookContent == null)
            {
                FileModel file = await SaveFileToStorage(book, wordDocument, cancellationToken);
                await _bookRepository.AddBookContent(command.BookId, file.Id, book.Language, MimeTypes.MsWord, cancellationToken);
            }
            else
            {
               await UpdateFileInStorage(book, bookContent.FileId, wordDocument, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<FileModel> SaveFileToStorage(BookModel book, byte[] wordDocument, CancellationToken cancellationToken)
        {
            var fileName = $"{book.Title.ToSafeFilename()}.docx";
            var url = await _fileStorage.StoreFile($"books/{book.Id}/{fileName}", wordDocument, cancellationToken);
            var file = await _fileRepository.AddFile(new FileModel
            {
                FilePath = url,
                MimeType = MimeTypes.MsWord,
                FileName = fileName,
                IsPublic = false
            }, cancellationToken);
            return file;
        }

        private async Task UpdateFileInStorage(BookModel book, long fileId, byte[] file, CancellationToken cancellationToken)
        {
            var fileName = $"{book.Title.ToSafeFilename()}.docx";
            var existingDocx = await _fileRepository.GetFileById(fileId, cancellationToken);
            if (existingDocx != null && !string.IsNullOrWhiteSpace(existingDocx.FilePath))
            {
                await _fileStorage.TryDeleteFile(existingDocx.FilePath, cancellationToken);
            }

            existingDocx.FilePath = await _fileStorage.StoreFile($"books/{book.Id}/{fileName}", file, cancellationToken);

            await _fileRepository.UpdateFile(existingDocx, cancellationToken);
        }

        private char[] pageBreakSymbols = new char[] { '۔', ':', '“', '"', '\'', '!' };

        private string CombinePages(IEnumerable<BookPageModel> pages)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var page in pages)
            {
                if (string.IsNullOrWhiteSpace(page.Text))
                {
                    continue;
                }

                var separator = " ";
                var finalText = page.Text.Trim();
                var lastCharacter = finalText.Last();

                if (pageBreakSymbols.Contains(lastCharacter))
                {
                    separator = Environment.NewLine;
                }

                builder.Append(separator);
                builder.Append(finalText);
            }

            return builder.ToString().TrimStart();
        }
    }
}
