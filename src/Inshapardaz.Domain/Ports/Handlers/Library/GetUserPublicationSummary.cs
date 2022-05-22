using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetUserPublicationSummary : LibraryBaseQuery<IEnumerable<UserPageSummaryItem>>
    {
        public GetUserPublicationSummary(int libraryId, int accountId)
            : base(libraryId)
        {
            AccountId = accountId;
        }

        public int PageSize { get; private set; }
        public int AccountId { get; set; }
    }

    public class GetUserPublicationSummaryHandler : QueryHandlerAsync<GetUserPublicationSummary, IEnumerable<UserPageSummaryItem>>
    {
        private readonly IBookPageRepository _bookPageRepository;

        public GetUserPublicationSummaryHandler(IBookPageRepository bookPageRepository)
        {
            _bookPageRepository = bookPageRepository;
        }

        public override async Task<IEnumerable<UserPageSummaryItem>> ExecuteAsync(GetUserPublicationSummary query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _bookPageRepository.GetUserPageSummary(query.LibraryId, query.AccountId, cancellationToken);
        }
    }
}
