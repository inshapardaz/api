using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Article;

public class GetArticlesQuery(int libraryId, int pageNumber, int pageSize, int? accountId)
    : LibraryBaseQuery<Page<ArticleModel>>(libraryId)
{
    public int PageNumber { get; private set; } = pageNumber;

    public int PageSize { get; private set; } = pageSize;
    public int? AccountId { get; } = accountId;
    public string Query { get; set; }

    public ArticleSortByType SortBy { get; set; }

    public ArticleFilter Filter { get; set; }
    public SortDirection SortDirection { get; set; }

}

public class GetArticlesQueryHandler(IArticleRepository articleRepository)
    : QueryHandlerAsync<GetArticlesQuery, Page<ArticleModel>>
{
    [LibraryAuthorize(1)]
    public override async Task<Page<ArticleModel>> ExecuteAsync(GetArticlesQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var articles = await articleRepository.GetArticles(
            command.LibraryId,
            command.Query,
            command.PageNumber,
            command.PageSize,
            command.AccountId,
            command.Filter,
            command.SortBy,
            command.SortDirection,
            cancellationToken);

        return articles;
    }
}
