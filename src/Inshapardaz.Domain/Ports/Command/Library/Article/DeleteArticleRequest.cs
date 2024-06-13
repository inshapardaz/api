using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Org.BouncyCastle.Asn1.Cms;
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
    private readonly IAmACommandProcessor _commandProcessor;

    public DeleteArticleRequestHandler(IArticleRepository articleRepository, IAmACommandProcessor commandProcessor)
    {
        _articleRepository = articleRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteArticleRequest> HandleAsync(DeleteArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var article = await _articleRepository.GetArticle(command.LibraryId, command.ArticleId, cancellationToken);
        if (article is not null)
        {
            foreach (var content in article.Contents)
            {
                await _commandProcessor.SendAsync(new DeleteTextFileCommand(content.FileId), cancellationToken: cancellationToken);
            }

            if (article.ImageId.HasValue)
            {
                await _commandProcessor.SendAsync(new DeleteFileCommand(article.ImageId), cancellationToken: cancellationToken);
            }

            await _articleRepository.DeleteArticle(command.LibraryId, command.ArticleId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
