using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Article;

public class UpdateIssueArticleRequest(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int sequenceNumber,
    IssueArticleModel article)
    : LibraryBaseCommand(libraryId)
{
    public RequestResult Result { get; set; } = new RequestResult();
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public int SequenceNumber { get; } = sequenceNumber;
    public IssueArticleModel Article { get; } = article;

    public class RequestResult
    {
        public IssueArticleModel Article { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateArticleRequestHandler(IIssueArticleRepository articleRepository, IIssueRepository issueRepository)
    : RequestHandlerAsync<UpdateIssueArticleRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssueArticleRequest> HandleAsync(UpdateIssueArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await articleRepository.GetIssueArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

        if (result == null)
        {
            var issue = await issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);

            if (issue == null)
            {
                throw new BadRequestException();
            }

            var article = command.Article;
            article.Id = default;
            command.Result.Article = await articleRepository.AddIssueArticle(command.LibraryId, command.PeriodicalId, issue.VolumeNumber, issue.IssueNumber, article, cancellationToken);
            command.Result.HasAddedNew = true;
        }
        else
        {
            command.Article.Id = result.Id;
            command.Result.Article = await articleRepository.UpdateIssueArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.Article, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
