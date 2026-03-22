using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Article;

public class DeleteArticleContentRequest(int libraryId, int articleId, string language) : LibraryBaseCommand(libraryId)
{
    public int ArticleId { get; } = articleId;
    public string Language { get; } = language;
}

public class DeleteArticleContentRequestHandler(
    IArticleRepository articleRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<DeleteArticleContentRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteArticleContentRequest> HandleAsync(DeleteArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var content = await articleRepository.GetArticleContent(command.LibraryId, command.ArticleId, command.Language, cancellationToken);

        if (content != null)
        {
            await commandProcessor.SendAsync(new DeleteTextFileCommand(content.FileId), cancellationToken: cancellationToken);
            await articleRepository.DeleteArticleContent(command.LibraryId, command.ArticleId, command.Language, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
