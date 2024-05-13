using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Ports.Command;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Article
{
    public class RemoveArticleFromFavoriteRequest : LibraryBaseCommand
    {
        public RemoveArticleFromFavoriteRequest(int libraryId, long articleId, int? accountId)
            : base(libraryId)
        {
            ArticleId = articleId;
            AccountId = accountId;
        }

        public long ArticleId { get; set; }
        public int? AccountId { get; }
    }

    public class RemoveArticleFromFavoriteRequestHandler 
        : RequestHandlerAsync<RemoveArticleFromFavoriteRequest>
    {
        private readonly IArticleRepository _articleRepository;

        public RemoveArticleFromFavoriteRequestHandler(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        [LibraryAuthorize(1)]
        public override async Task<RemoveArticleFromFavoriteRequest> HandleAsync(RemoveArticleFromFavoriteRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _articleRepository.RemoveArticleFromFavorites(command.LibraryId, command.AccountId.Value, command.ArticleId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
