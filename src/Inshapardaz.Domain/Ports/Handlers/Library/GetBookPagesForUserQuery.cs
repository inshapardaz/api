using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetBookPagesForUserQuery : LibraryBaseQuery<Page<BookPageModel>>
    {
        public GetBookPagesForUserQuery(int libraryId, int accountId, int pageNumber, int pageSize)
            : base(libraryId)
        {
            AccountId = accountId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
        public EditingStatus StatusFilter { get; set; }
        public int AccountId { get; set; }
    }

    public class GetBookPagesForUserQueryHandler : QueryHandlerAsync<GetBookPagesForUserQuery, Page<BookPageModel>>
    {
        private readonly IBookPageRepository _bookPageRepository;

        public GetBookPagesForUserQueryHandler(IBookPageRepository bookPageRepository)
        {
            _bookPageRepository = bookPageRepository;
        }

        public override async Task<Page<BookPageModel>> ExecuteAsync(GetBookPagesForUserQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return  await _bookPageRepository.GetPagesByUser(query.LibraryId, query.AccountId, query.StatusFilter, query.PageNumber, query.PageSize, cancellationToken);
        }
    }
}
