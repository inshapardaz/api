using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetChaptersByBookQuery : LibraryAuthorisedQuery<IEnumerable<ChapterModel>>
    {
        public GetChaptersByBookQuery(int libraryId, int bookId, Guid userId)
            : base(libraryId, userId)
        {
            BookId = bookId;
        }

        public int BookId { get; set; }
    }

    public class GetChaptersByBookQuerytHandler : QueryHandlerAsync<GetChaptersByBookQuery, IEnumerable<ChapterModel>>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IChapterRepository _chapterRepository;

        public GetChaptersByBookQuerytHandler(IBookRepository bookRepository, IChapterRepository chapterRepository)
        {
            _bookRepository = bookRepository;
            _chapterRepository = chapterRepository;
        }

        public override async Task<IEnumerable<ChapterModel>> ExecuteAsync(GetChaptersByBookQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.UserId, cancellationToken);
            if (book == null) return null;

            var chapters = await _chapterRepository.GetChaptersByBook(command.BookId, cancellationToken);

            foreach (var chapter in chapters)
            {
                var contents = await _chapterRepository.GetChapterContents(command.BookId, chapter.Id, cancellationToken);
                chapter.Contents = contents;
            }

            return chapters;
        }
    }
}
