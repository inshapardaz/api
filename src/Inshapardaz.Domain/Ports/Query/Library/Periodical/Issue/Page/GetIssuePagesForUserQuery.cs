using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue.Page;

public class GetIssuePagesForUserQuery(int libraryId, int accountId, int pageNumber, int pageSize)
    : LibraryBaseQuery<Page<IssuePageModel>>(libraryId)
{
    public int PageNumber { get; private set; } = pageNumber;
    public int PageSize { get; private set; } = pageSize;
    public EditingStatus StatusFilter { get; set; }
    public int AccountId { get; set; } = accountId;
}

public class GetIssuePagesForUserQueryHandler(IIssuePageRepository issuePageRepository)
    : QueryHandlerAsync<GetIssuePagesForUserQuery, Page<IssuePageModel>>
{
    public override async Task<Page<IssuePageModel>> ExecuteAsync(GetIssuePagesForUserQuery query, CancellationToken cancellationToken = new CancellationToken()) => await issuePageRepository.GetPagesByUser(query.LibraryId, query.AccountId, query.StatusFilter, query.PageNumber, query.PageSize, cancellationToken);
}
