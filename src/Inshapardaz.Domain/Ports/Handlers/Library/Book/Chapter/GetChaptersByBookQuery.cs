using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Book.Chapter
{
    public class GetChaptersByBookQuery : LibraryBaseQuery<IEnumerable<ChapterModel>>
    {
        public GetChaptersByBookQuery(int libraryId, int bookId, int? accountId)
            : base(libraryId)
        {
            BookId = bookId;
            AccountId = accountId;
        }

        public int BookId { get; set; }
        public int? AccountId { get; }
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
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.AccountId, cancellationToken);
            if (book == null) return null;

            return await _chapterRepository.GetChaptersByBook(command.LibraryId, command.BookId, cancellationToken);
        }
    }
}
