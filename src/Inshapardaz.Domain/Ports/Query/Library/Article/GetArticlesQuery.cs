using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Article;

public class GetArticlesQuery : LibraryBaseQuery<Page<ArticleModel>>
{
    public GetArticlesQuery(int libraryId, int pageNumber, int pageSize, int? accountId)
        : base(libraryId)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        AccountId = accountId;
    }

    public int PageNumber { get; private set; }

    public int PageSize { get; private set; }
    public int? AccountId { get; }
    public string Query { get; set; }

    public ArticleSortByType SortBy { get; set; }

    public ArticleFilter Filter { get; set; }
    public SortDirection SortDirection { get; set; }

}

public class GetArticlesQueryHandler : QueryHandlerAsync<GetArticlesQuery, Page<ArticleModel>>
{
    private readonly IArticleRepository _articleRepository;

    public GetArticlesQueryHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    [LibraryAuthorize(1)]
    public override async Task<Page<ArticleModel>> ExecuteAsync(GetArticlesQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var articles = await _articleRepository.GetArticles(
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
