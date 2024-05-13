using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Ports.Command;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Article
{
    public class AddArticleToFavoriteRequest : LibraryBaseCommand
    {
        public AddArticleToFavoriteRequest(int libraryId, long articleId, int? accountId)
            : base(libraryId)
        {
            ArticleId = articleId;
            AccountId = accountId;
        }

        public long ArticleId { get; }
        public int? AccountId { get; }
    }

    public class AddArticleToFavoriteRequestHandler : RequestHandlerAsync<AddArticleToFavoriteRequest>
    {
        private readonly IArticleRepository _articleRepository;

        public AddArticleToFavoriteRequestHandler(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        [LibraryAuthorize(1)]
        public override async Task<AddArticleToFavoriteRequest> HandleAsync(AddArticleToFavoriteRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var article = await _articleRepository.GetArticle(command.LibraryId, command.ArticleId, cancellationToken);
            if (article != null)
            {
                await _articleRepository.AddArticleToFavorites(command.LibraryId, command.AccountId, command.ArticleId, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
