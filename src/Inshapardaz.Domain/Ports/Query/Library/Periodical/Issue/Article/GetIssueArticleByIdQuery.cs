using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue.Article;

public class GetIssueArticleByIdQuery : LibraryBaseQuery<IssueArticleModel>
{
    public GetIssueArticleByIdQuery(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        SequenceNumber = sequenceNumber;
    }

    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
    public int SequenceNumber { get; }
}

public class GetArticleByIdQueryHandler : QueryHandlerAsync<GetIssueArticleByIdQuery, IssueArticleModel>
{
    private readonly IIssueArticleRepository _articleRepository;

    public GetArticleByIdQueryHandler(IIssueArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public override async Task<IssueArticleModel> ExecuteAsync(GetIssueArticleByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var article = await _articleRepository.GetIssueArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

        if (article != null)
        {
            article.PreviousArticle = await _articleRepository.GetIssueArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber - 1, cancellationToken);
            article.NextArticle = await _articleRepository.GetIssueArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber + 1, cancellationToken);
        }

        return article;
    }
}
