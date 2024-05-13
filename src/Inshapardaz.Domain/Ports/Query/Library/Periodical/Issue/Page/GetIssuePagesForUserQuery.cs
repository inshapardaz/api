using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue.Page;

public class GetIssuePagesForUserQuery : LibraryBaseQuery<Page<IssuePageModel>>
{
    public GetIssuePagesForUserQuery(int libraryId, int accountId, int pageNumber, int pageSize)
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

public class GetIssuePagesForUserQueryHandler : QueryHandlerAsync<GetIssuePagesForUserQuery, Page<IssuePageModel>>
{
    private readonly IIssuePageRepository _issuePageRepository;

    public GetIssuePagesForUserQueryHandler(IIssuePageRepository issuePageRepository)
    {
        _issuePageRepository = issuePageRepository;
    }

    public override async Task<Page<IssuePageModel>> ExecuteAsync(GetIssuePagesForUserQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        return await _issuePageRepository.GetPagesByUser(query.LibraryId, query.AccountId, query.StatusFilter, query.PageNumber, query.PageSize, cancellationToken);
    }
}
