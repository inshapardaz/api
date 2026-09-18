using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue;

// No [LibraryAuthorize] - the average rating is part of the issue's public summary, same
// visibility as the issue itself (enforced by GetIssueByIdQuery upstream of this).
public class GetIssueRatingSummaryQuery(int libraryId, int periodicalId, int volumeNumber, int issueNumber)
    : LibraryBaseQuery<RatingSummaryModel>(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
}

public class GetIssueRatingSummaryQueryHandler(IIssueRepository issueRepository)
    : QueryHandlerAsync<GetIssueRatingSummaryQuery, RatingSummaryModel>
{
    public override async Task<RatingSummaryModel> ExecuteAsync(GetIssueRatingSummaryQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await issueRepository.GetIssue(query.LibraryId, query.PeriodicalId, query.VolumeNumber, query.IssueNumber, cancellationToken);

        return issue == null
            ? new RatingSummaryModel()
            : await issueRepository.GetIssueRatingSummary(query.LibraryId, issue.Id, cancellationToken);
    }
}
