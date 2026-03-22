using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Article;

public class DeleteArticleRequest(int libraryId, int articleId) : LibraryBaseCommand(libraryId)
{
    public int ArticleId { get; } = articleId;
}

public class DeleteArticleRequestHandler(IArticleRepository articleRepository, IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<DeleteArticleRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteArticleRequest> HandleAsync(DeleteArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var article = await articleRepository.GetArticle(command.LibraryId, command.ArticleId, cancellationToken);
        if (article is not null)
        {
            foreach (var content in article.Contents)
            {
                await commandProcessor.SendAsync(new DeleteTextFileCommand(content.FileId), cancellationToken: cancellationToken);
            }

            if (article.ImageId.HasValue)
            {
                await commandProcessor.SendAsync(new DeleteFileCommand(article.ImageId), cancellationToken: cancellationToken);
            }

            await articleRepository.DeleteArticle(command.LibraryId, command.ArticleId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
