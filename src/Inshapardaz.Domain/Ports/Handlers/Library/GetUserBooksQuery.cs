using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetUserBooksQuery : LibraryBaseQuery<Page<BookModel>>
    {
        public GetUserBooksQuery(int libraryId, int accountId, int pageNumber, int pageSize)
            : base(libraryId)
        {
            AccountId = accountId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
        public BookStatuses StatusFilter { get; set; }
        public int AccountId { get; set; }
    }

    public class GetUserBooksQueryHandler : QueryHandlerAsync<GetUserBooksQuery, Page<BookModel>>
    {
        private readonly IBookRepository _bookRepository;

        public GetUserBooksQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<Page<BookModel>> ExecuteAsync(GetUserBooksQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _bookRepository.GetBooksByUser(query.LibraryId, query.AccountId, query.PageNumber, query.PageSize, query.StatusFilter, BookSortByType.Title, SortDirection.Ascending, cancellationToken);
        }
    }
}
