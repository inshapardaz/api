using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Query;
using Inshapardaz.Domain.Ports.Query.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Article;

public class GetArticleByIdQuery : LibraryBaseQuery<ArticleModel>
{
    public GetArticleByIdQuery(int libraryId, int articleId)
        : base(libraryId)
    {
        ArticleId = articleId;
    }

    public int ArticleId { get; }
}

public class GetArticleByIdQueryHandler : QueryHandlerAsync<GetArticleByIdQuery, ArticleModel>
{
    private readonly IArticleRepository _articleRepository;

    public GetArticleByIdQueryHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    [LibraryAuthorize(1)]
    public override async Task<ArticleModel> ExecuteAsync(GetArticleByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        return await _articleRepository.GetArticle(command.LibraryId, command.ArticleId, cancellationToken);
    }
}
