using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Article;

public class UpdateIssueArticleRequest : LibraryBaseCommand
{
    public UpdateIssueArticleRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, IssueArticleModel article)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        ArticleId = articleId;
        Article = article;
    }

    public RequestResult Result { get; set; } = new RequestResult();
    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
    public int ArticleId { get; }
    public IssueArticleModel Article { get; }

    public class RequestResult
    {
        public IssueArticleModel Article { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateArticleRequestHandler : RequestHandlerAsync<UpdateIssueArticleRequest>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IIssueArticleRepository _articleRepository;

    public UpdateArticleRequestHandler(IIssueArticleRepository articleRepository, IIssueRepository issueRepository)
    {
        _articleRepository = articleRepository;
        _issueRepository = issueRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateIssueArticleRequest> HandleAsync(UpdateIssueArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await _articleRepository.GetArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ArticleId, cancellationToken);

        if (result == null)
        {
            var issue = await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);

            if (issue == null)
            {
                throw new BadRequestException();
            }

            var article = command.Article;
            article.Id = default;
            command.Result.Article = await _articleRepository.AddArticle(command.LibraryId, command.PeriodicalId, issue.VolumeNumber, issue.IssueNumber, article, cancellationToken);
            command.Result.HasAddedNew = true;
        }
        else
        {
            await _articleRepository.UpdateArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.Article, cancellationToken);
            command.Article.Id = result.Id;
            command.Result.Article = command.Article;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
