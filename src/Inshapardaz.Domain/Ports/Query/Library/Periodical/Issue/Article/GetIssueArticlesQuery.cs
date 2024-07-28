using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue.Article;

public class GetIssueArticlesQuery : LibraryBaseQuery<IEnumerable<IssueArticleModel>>
{
    public GetIssueArticlesQuery(int libraryId, int periodicalId, int volumeNumber, int issueNumber)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
    }

    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
}

public class GetArticlesByIssueQuerytHandler : QueryHandlerAsync<GetIssueArticlesQuery, IEnumerable<IssueArticleModel>>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IIssueArticleRepository _articleRepository;

    public GetArticlesByIssueQuerytHandler(IIssueRepository issueRepository, IIssueArticleRepository articleRepository)
    {
        _issueRepository = issueRepository;
        _articleRepository = articleRepository;
    }

    public override async Task<IEnumerable<IssueArticleModel>> ExecuteAsync(GetIssueArticlesQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issue = await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
        if (issue == null)
        {
            return null;
        }

        return await _articleRepository.GetIssueArticlesByIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
    }
}
