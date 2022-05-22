using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdateArticleRequest : LibraryBaseCommand
    {
        public UpdateArticleRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, ArticleModel article)
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
        public ArticleModel Article { get; }

        public class RequestResult
        {
            public ArticleModel Article { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateArticleRequestHandler : RequestHandlerAsync<UpdateArticleRequest>
    {
        private readonly IArticleRepository _articleRepository;

        public UpdateArticleRequestHandler(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public override async Task<UpdateArticleRequest> HandleAsync(UpdateArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _articleRepository.GetArticleById(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ArticleId, cancellationToken);

            if (result == null)
            {
                var article = command.Article;
                article.Id = default(int);
                command.Result.Article = await _articleRepository.AddArticle(command.LibraryId, command.PeriodicalId, result.Id, article, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                await _articleRepository.UpdateArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.Article, cancellationToken);
                command.Result.Article = command.Article;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
