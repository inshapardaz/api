using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Article
{
    public class AddArticleRequest : LibraryBaseCommand
    {
        public AddArticleRequest(int libraryId, ArticleModel article)
            : base(libraryId)
        {
            Article = article;
        }

        public ArticleModel Result { get; set; }
        public ArticleModel Article { get; }
    }

    public class AddArticleRequestHandler : RequestHandlerAsync<AddArticleRequest>
    {
        private readonly IArticleRepository _articleRepository;

        public AddArticleRequestHandler(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public override async Task<AddArticleRequest> HandleAsync(AddArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _articleRepository.AddArticle(command.LibraryId, command.Article, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
