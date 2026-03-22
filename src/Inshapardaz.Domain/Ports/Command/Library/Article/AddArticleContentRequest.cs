using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Article;

public class AddArticleContentRequest(int libraryId) : LibraryBaseCommand(libraryId)
{
    public ArticleContentModel Content { get; set; }

    public ArticleContentModel Result { get; set; }
}

public class AddArticleContentRequestHandler(
    IArticleRepository articleRepository,
    ILibraryRepository libraryRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<AddArticleContentRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddArticleContentRequest> HandleAsync(AddArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (string.IsNullOrWhiteSpace(command.Content.Language))
        {
            var library = await libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
            if (library == null)
            {
                throw new BadRequestException();
            }

            command.Content.Language = library.Language;
        }

        var article = await articleRepository.GetArticle(command.LibraryId, command.Content.ArticleId, cancellationToken);

        if (article == null)
        {
            throw new BadRequestException();
        }

        var fileName = FilePathHelper.GetArticleContentFileName(command.Content.Language);
        var saveFileCommand = new SaveTextFileCommand(
            fileName,
            FilePathHelper.GetArticleContentPath(command.Content.ArticleId, fileName),
            command.Content.Text)
        {
            MimeType = MimeTypes.Markdown
        };

        await commandProcessor.SendAsync(saveFileCommand, cancellationToken: cancellationToken);
        command.Content.FileId = saveFileCommand.Result.Id;

        command.Result = await articleRepository.AddArticleContent(
            command.LibraryId,
            command.Content,
            cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
