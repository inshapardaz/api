using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library;

public class GetUserPublicationSummary(int libraryId, int accountId)
    : LibraryBaseQuery<IEnumerable<UserPageSummaryItem>>(libraryId)
{
    public int PageSize { get; private set; }
    public int AccountId { get; set; } = accountId;
}

public class GetUserPublicationSummaryHandler(IBookPageRepository bookPageRepository)
    : QueryHandlerAsync<GetUserPublicationSummary, IEnumerable<UserPageSummaryItem>>
{
    public override async Task<IEnumerable<UserPageSummaryItem>> ExecuteAsync(GetUserPublicationSummary query, CancellationToken cancellationToken = new CancellationToken()) => await bookPageRepository.GetUserPageSummary(query.LibraryId, query.AccountId, cancellationToken);
}
