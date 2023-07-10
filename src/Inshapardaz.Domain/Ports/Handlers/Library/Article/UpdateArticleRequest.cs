using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Article
{
    public class UpdateArticleRequest : LibraryBaseCommand
    {
        public UpdateArticleRequest(int libraryId, int articleId, ArticleModel article)
            : base(libraryId)
        {
            ArticleId = articleId;
            Article = article;
        }

        public RequestResult Result { get; set; } = new RequestResult();
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
            var result = await _articleRepository.GetArticle(command.LibraryId, command.ArticleId, cancellationToken);

            if (result == null)
            {
                var article = command.Article;
                article.Id = default;
                command.Result.Article = await _articleRepository.AddArticle(command.LibraryId, article, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                await _articleRepository.UpdateArticle(command.LibraryId, command.ArticleId, command.Article, cancellationToken);
                command.Article.Id = result.Id;
                command.Result.Article = command.Article;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
