using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Article;

public class DeleteArticleRequest : LibraryBaseCommand
{
    public DeleteArticleRequest(int libraryId, int articleId)
        : base(libraryId)
    {
        ArticleId = articleId;
    }

    public int ArticleId { get; }
}

public class DeleteArticleRequestHandler : RequestHandlerAsync<DeleteArticleRequest>
{
    private readonly IArticleRepository _articleRepository;

    public DeleteArticleRequestHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteArticleRequest> HandleAsync(DeleteArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        await _articleRepository.DeleteArticle(command.LibraryId, command.ArticleId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
