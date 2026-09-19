using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue;

public class GetIssueRatingQuery(int libraryId, int accountId, int periodicalId, int volumeNumber, int issueNumber)
    : LibraryBaseQuery<RatingModel>(libraryId)
{
    public int AccountId { get; } = accountId;
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
}

public class GetIssueRatingQueryHandler(IIssueRepository issueRepository)
    : QueryHandlerAsync<GetIssueRatingQuery, RatingModel>
{
    [LibraryAuthorize(1, Role.Reader, Role.Writer, Role.Admin, Role.LibraryAdmin)]
    public override async Task<RatingModel> ExecuteAsync(GetIssueRatingQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await issueRepository.GetIssue(query.LibraryId, query.PeriodicalId, query.VolumeNumber, query.IssueNumber, cancellationToken);

        return issue == null
            ? null
            : await issueRepository.GetIssueRating(query.LibraryId, query.AccountId, issue.Id, cancellationToken);
    }
}
