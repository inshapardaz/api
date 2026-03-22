using Inshapardaz.Domain.Adapters.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Article;

public class RemoveArticleFromFavoriteRequest(int libraryId, long articleId, int? accountId)
    : LibraryBaseCommand(libraryId)
{
    public long ArticleId { get; set; } = articleId;
    public int? AccountId { get; } = accountId;
}

public class RemoveArticleFromFavoriteRequestHandler(IArticleRepository articleRepository)
    : RequestHandlerAsync<RemoveArticleFromFavoriteRequest>
{
    [LibraryAuthorize(1)]
    public override async Task<RemoveArticleFromFavoriteRequest> HandleAsync(RemoveArticleFromFavoriteRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        await articleRepository.RemoveArticleFromFavorites(command.LibraryId, command.AccountId.Value, command.ArticleId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
