using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Article;

public class UpdateArticleImageRequest(int libraryId, long articleId, int? accountId) : LibraryBaseCommand(libraryId)
{
    public long ArticleId { get; } = articleId;
    public int? AccountId { get; } = accountId;
    public FileModel Image { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public FileModel File { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateArticleImageRequestHandler(
    IArticleRepository articleRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<UpdateArticleImageRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateArticleImageRequest> HandleAsync(UpdateArticleImageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var article = await articleRepository.GetArticle(command.LibraryId, command.ArticleId, cancellationToken);

        if (article == null)
        {
            throw new NotFoundException();
        }

        var fileName = FilePathHelper.GetArticleImageFileName(command.Image.FileName);
        var filePath = FilePathHelper.GetArticleImagePath(command.ArticleId, fileName);

        var saveContentCommand = new SaveFileCommand(fileName, filePath, command.Image.Contents)
        {
            MimeType = command.Image.MimeType,
            ExistingFileId = article.ImageId
        };

        await commandProcessor.SendAsync(saveContentCommand, cancellationToken: cancellationToken);
        command.Result.File = saveContentCommand.Result;
                
        if (!article.ImageId.HasValue)
        {
            await articleRepository.UpdateArticleImage(command.LibraryId, command.ArticleId, command.Result.File.Id, cancellationToken);
            command.Result.HasAddedNew = true;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
