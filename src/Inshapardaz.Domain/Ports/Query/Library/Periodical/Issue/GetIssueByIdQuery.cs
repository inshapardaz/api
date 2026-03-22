using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue;

public class GetIssueByIdQuery(int libraryId, int periodicalId, int volumeNumber, int issueNumber)
    : LibraryBaseQuery<IssueModel>(libraryId)
{
    public int PeriodicalId { get; private set; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
}

public class GetIssueByIdQueryHandler(IIssueRepository issueRepository)
    : QueryHandlerAsync<GetIssueByIdQuery, IssueModel>
{
    public override async Task<IssueModel> ExecuteAsync(GetIssueByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);

        if (issue != null)
        {
            var status = (await issueRepository.GetIssuePageSummary(command.LibraryId, [issue.Id], cancellationToken)).FirstOrDefault();

            if (status != null)
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
            
            var contents = await issueRepository.GetIssueContents(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);

            issue.Contents = contents.ToList();
        }

        return issue;
    }
}
