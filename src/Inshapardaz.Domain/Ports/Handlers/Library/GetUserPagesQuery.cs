using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetUserPagesQuery : LibraryBaseQuery<Page<BookPageModel>>
    {
        public GetUserPagesQuery(int libraryId, int accountId, int pageNumber, int pageSize)
            : base(libraryId)
        {
            AccountId = accountId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
        public PageStatuses StatusFilter { get; set; }
        public int AccountId { get; set; }
    }

    public class GetUserPagesQueryHandler : QueryHandlerAsync<GetUserPagesQuery, Page<BookPageModel>>
    {
        private readonly IBookPageRepository _bookPageRepository;

        public GetUserPagesQueryHandler(IBookPageRepository bookPageRepository)
        {
            _bookPageRepository = bookPageRepository;
        }

        public override async Task<Page<BookPageModel>> ExecuteAsync(GetUserPagesQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return  await _bookPageRepository.GetPagesByUser(query.LibraryId, query.AccountId, query.StatusFilter, query.PageNumber, query.PageSize, cancellationToken);
        }
    }
}
