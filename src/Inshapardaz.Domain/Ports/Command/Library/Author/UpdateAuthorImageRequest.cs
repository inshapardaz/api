using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Author;

public class UpdateAuthorImageRequest : LibraryBaseCommand
{
    public UpdateAuthorImageRequest(int libraryId, int authorId)
        : base(libraryId)
    {
        AuthorId = authorId;
    }

    public int AuthorId { get; }

    public FileModel Image { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public FileModel File { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateAuthorImageRequestHandler : RequestHandlerAsync<UpdateAuthorImageRequest>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public UpdateAuthorImageRequestHandler(IAuthorRepository authorRepository, 
        IAmACommandProcessor commandProcessor)
    {
        _authorRepository = authorRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateAuthorImageRequest> HandleAsync(UpdateAuthorImageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var author = await _authorRepository.GetAuthorById(command.LibraryId, command.AuthorId, cancellationToken);

        if (author == null)
        {
            throw new NotFoundException();
        }

        var fileName = FilePathHelper.GetAuthorImageFileName(command.Image.FileName);
        var filePath = FilePathHelper.GetAuthorImagePath(command.AuthorId, fileName);

        var saveContentCommand = new SaveFileCommand(fileName, filePath, command.Image.Contents)
        {
            MimeType = command.Image.MimeType,
            ExistingFileId = author.ImageId,
            IsPublic = true
        };

        await _commandProcessor.SendAsync(saveContentCommand, cancellationToken: cancellationToken);
        command.Result.File = saveContentCommand.Result;

        if (!author.ImageId.HasValue)
        {
            await _authorRepository.UpdateAuthorImage(command.LibraryId, command.AuthorId, command.Result.File.Id, cancellationToken);
            command.Result.HasAddedNew = true;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
