using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue.Article;

public class GetIssueArticleByIdQuery(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int sequenceNumber)
    : LibraryBaseQuery<IssueArticleModel>(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public int SequenceNumber { get; } = sequenceNumber;
}

public class GetArticleByIdQueryHandler(IIssueArticleRepository articleRepository)
    : QueryHandlerAsync<GetIssueArticleByIdQuery, IssueArticleModel>
{
    public override async Task<IssueArticleModel> ExecuteAsync(GetIssueArticleByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var article = await articleRepository.GetIssueArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

        if (article != null)
        {
            article.PreviousArticle = await articleRepository.GetIssueArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber - 1, cancellationToken);
            article.NextArticle = await articleRepository.GetIssueArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber + 1, cancellationToken);
        }

        return article;
    }
}
