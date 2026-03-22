using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue.Article;

public class GetIssueArticlesQuery(int libraryId, int periodicalId, int volumeNumber, int issueNumber)
    : LibraryBaseQuery<IEnumerable<IssueArticleModel>>(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
}

public class GetArticlesByIssueQuerytHandler(
    IIssueRepository issueRepository,
    IIssueArticleRepository articleRepository)
    : QueryHandlerAsync<GetIssueArticlesQuery, IEnumerable<IssueArticleModel>>
{
    public override async Task<IEnumerable<IssueArticleModel>> ExecuteAsync(GetIssueArticlesQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        if (issue == null)
        {
            return null;
        }

        return await articleRepository.GetIssueArticlesByIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
    }
}
