using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue;

public class GetIssuesQuery(int libraryId, int periodicalId, int pageNumber, int pageSize)
    : LibraryBaseQuery<Page<IssueModel>>(libraryId)
{
    public int PeriodicalId { get; private set; } = periodicalId;

    public int PageNumber { get; private set; } = pageNumber;

    public int PageSize { get; private set; } = pageSize;
    public IssueFilter Filter { get; set; }
    public IssueSortByType SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
}

public class GetIssuesRequestHandler(
    IIssueRepository issueRepository,
    IPeriodicalRepository periodicalRepository,
    IFileRepository fileRepository)
    : QueryHandlerAsync<GetIssuesQuery, Page<IssueModel>>
{
    public override async Task<Page<IssueModel>> ExecuteAsync(GetIssuesQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var periodical = await periodicalRepository.GetPeriodicalById(command.LibraryId, command.PeriodicalId, cancellationToken);
        if (periodical == null) return null;
        var issues = await issueRepository.GetIssues(command.LibraryId, command.PeriodicalId, command.PageNumber, command.PageSize, command.Filter, command.SortBy, command.SortDirection, cancellationToken);
        
        var statuses = (await issueRepository.GetIssuePageSummary(command.LibraryId, 
            issues.Data.Select(x => x.Id).ToArray(), 
            cancellationToken));
        foreach (var status in statuses)
        {
            var issue = issues.Data.SingleOrDefault(b => b.Id == status.BookId);
            if (issue != null)
            {
                issue.PageStatus = status.Statuses;
                if (status.Statuses.Any(s => s.Status == EditingStatus.Completed))
                {
                    decimal completedPages = status.Statuses.Single(s => s.Status == EditingStatus.Completed).Count;
                    issue.Progress = completedPages / issue.PageCount;
                }
                else
                {
                    issue.Progress = 0.0M;
                }
            }
        }
        foreach (var issue in issues.Data)
        {
            
            if (issue != null && issue.ImageUrl == null && issue.ImageId.HasValue)
            {
                issue.ImageUrl = await ImageHelper.TryConvertToPublicFile(issue.ImageId.Value, fileRepository, cancellationToken);
            }

            var contents = await issueRepository.GetIssueContents(command.LibraryId, issue.PeriodicalId, issue.VolumeNumber, issue.IssueNumber, cancellationToken);

            issue.Contents = contents.ToList();
        }

        return issues;
    }
}
