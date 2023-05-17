using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
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

        public PublishBookRequestHandler(IBookRepository bookRepository,
            IChapterRepository chapterRepository,
            IBookPageRepository bookPageRepository)
        {
            _bookRepository = bookRepository;
            _chapterRepository = chapterRepository;
            _bookPageRepository = bookPageRepository;
        }

        public override async Task<PublishBookRequest> HandleAsync(PublishBookRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
            var chapters = await _chapterRepository.GetChaptersByBook(command.LibraryId, command.BookId, cancellationToken);

            foreach (var chapter in chapters)
            {
                var pages = await _bookPageRepository.GetPagesByBookChapter(command.LibraryId, command.BookId, chapter.Id, cancellationToken);
                var finalText = CombinePages(pages);
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

            return await base.HandleAsync(command, cancellationToken);
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

            return builder.ToString();
        }
    }
}
