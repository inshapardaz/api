using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue.Page;

public class GetIssuePagesQuery(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int pageNumber,
    int pageSize)
    : LibraryBaseQuery<Page<IssuePageModel>>(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public int PageNumber { get; private set; } = pageNumber;
    public int PageSize { get; private set; } = pageSize;
    public EditingStatus StatusFilter { get; set; }
    public AssignmentFilter WriterAssignmentFilter { get; set; }
    public int? AccountId { get; set; }
    public AssignmentFilter ReviewerAssignmentFilter { get; set; }
}

public class GetIssuePagesQueryHandler(IIssuePageRepository issuePageRepository, IQueryProcessor queryProcessor)
    : QueryHandlerAsync<GetIssuePagesQuery, Page<IssuePageModel>>
{
    private readonly IQueryProcessor _queryProcessor = queryProcessor;

    public override async Task<Page<IssuePageModel>> ExecuteAsync(GetIssuePagesQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var pages = await issuePageRepository.GetPagesByIssue(query.LibraryId, query.PeriodicalId, query.VolumeNumber, query.IssueNumber, query.PageNumber, query.PageSize, query.StatusFilter, query.WriterAssignmentFilter, query.ReviewerAssignmentFilter, query.AccountId, cancellationToken);

        // foreach (var page in pages.Data)
        // {
        //     if (page.FileId.HasValue)
        //     { 
        //         page.Text = await _queryProcessor.ExecuteAsync(new GetTextFileQuery(page.FileId.Value), cancellationToken);
        //     }
        // }

        return pages;
    }
}
