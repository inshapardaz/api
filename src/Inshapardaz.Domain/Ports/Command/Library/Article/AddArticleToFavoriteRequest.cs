using Inshapardaz.Domain.Adapters.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Article;

public class AddArticleToFavoriteRequest(int libraryId, long articleId, int? accountId) : LibraryBaseCommand(libraryId)
{
    public long ArticleId { get; } = articleId;
    public int? AccountId { get; } = accountId;
}

public class AddArticleToFavoriteRequestHandler(IArticleRepository articleRepository)
    : RequestHandlerAsync<AddArticleToFavoriteRequest>
{
    [LibraryAuthorize(1)]
    public override async Task<AddArticleToFavoriteRequest> HandleAsync(AddArticleToFavoriteRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var article = await articleRepository.GetArticle(command.LibraryId, command.ArticleId, cancellationToken);
        if (article != null)
        {
            await articleRepository.AddArticleToFavorites(command.LibraryId, command.AccountId, command.ArticleId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
