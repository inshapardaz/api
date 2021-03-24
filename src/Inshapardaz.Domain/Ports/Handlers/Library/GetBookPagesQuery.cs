using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Lucene.Net.Search;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetBookPagesQuery : LibraryBaseQuery<Page<BookPageModel>>
    {
        public GetBookPagesQuery(int libraryId, int bookId, int pageNumber, int pageSize)
            : base(libraryId)
        {
            BookId = bookId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int BookId { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
        public PageStatuses StatusFilter { get; set; }
        public AssignmentFilter AssignmentFilter { get; set; }
        public int? AccountId { get; set; }
    }

    public class GetBookPagesQueryHandler : QueryHandlerAsync<GetBookPagesQuery, Page<BookPageModel>>
    {
        private readonly IBookPageRepository _bookPageRepository;
        private readonly IAccountRepository _accountRepository;

        public GetBookPagesQueryHandler(IBookPageRepository bookPageRepository, IAccountRepository accountRepository)
        {
            _bookPageRepository = bookPageRepository;
            _accountRepository = accountRepository;
        }

        public override async Task<Page<BookPageModel>> ExecuteAsync(GetBookPagesQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            var authors = await _bookPageRepository.GetPagesByBook(query.LibraryId, query.BookId, query.PageNumber, query.PageSize, query.StatusFilter, query.AssignmentFilter, query.AccountId, cancellationToken);

            return authors;
        }
    }
}
