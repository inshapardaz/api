using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Article;

public class UpdateArticleContentRequest(int libraryId) : LibraryBaseCommand(libraryId)
{
    public ArticleContentModel Content { get; set; }


    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public ArticleContentModel Content { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateArticleContentRequestHandler(
    IArticleRepository articleRepository,
    ILibraryRepository libraryRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<UpdateArticleContentRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateArticleContentRequest> HandleAsync(UpdateArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var article = await articleRepository.GetArticle(command.LibraryId, command.Content.ArticleId, cancellationToken);

        if (article == null)
        {
            throw new BadRequestException();
        }

        if (string.IsNullOrWhiteSpace(command.Content.Language))
        {
            var library = await libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
            if (library == null)
            {
                throw new BadRequestException();
            }

            command.Content.Language = library.Language;
        }

        var content = await articleRepository.GetArticleContent(command.LibraryId, command.Content.ArticleId, command.Content.Language, cancellationToken);

        var fileName = FilePathHelper.GetArticleContentFileName(command.Content.Language);
        var saveFileCommand = new SaveTextFileCommand(
            fileName,
            FilePathHelper.GetArticleContentPath(command.Content.ArticleId, fileName),
            command.Content.Text)
        {
            MimeType = MimeTypes.Markdown,
            ExistingFileId = content?.FileId
        }; 

        await commandProcessor.SendAsync(saveFileCommand, cancellationToken: cancellationToken);
        command.Content.FileId = saveFileCommand.Result.Id;

        if (content == null)
        {
            command.Result.Content = await articleRepository.AddArticleContent(
                command.LibraryId,
                command.Content,
                cancellationToken);
            command.Result.HasAddedNew = true;
        }
        else
        {
            command.Result.Content = await articleRepository.UpdateArticleContent(command.LibraryId,
                                                    command.Content,
                                                    cancellationToken);
        }

        if (command.Result.Content != null)
        {
            command.Result.Content.Text = command.Content.Text;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
