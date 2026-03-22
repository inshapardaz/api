using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Article;

public class GetArticleByIdQuery(int libraryId, int articleId) : LibraryBaseQuery<ArticleModel>(libraryId)
{
    public int ArticleId { get; } = articleId;
}

public class GetArticleByIdQueryHandler(IArticleRepository articleRepository)
    : QueryHandlerAsync<GetArticleByIdQuery, ArticleModel>
{
    [LibraryAuthorize(1)]
    public override async Task<ArticleModel> ExecuteAsync(GetArticleByIdQuery command, CancellationToken cancellationToken = new CancellationToken()) => await articleRepository.GetArticle(command.LibraryId, command.ArticleId, cancellationToken);
}
