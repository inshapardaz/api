using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical;

public class UpdatePeriodicalImageRequest : LibraryBaseCommand
{
    public UpdatePeriodicalImageRequest(int libraryId, int periodicalId)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
    }

    public int PeriodicalId { get; }

    public FileModel Image { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public FileModel File { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdatePeriodicalImageRequestHandler : RequestHandlerAsync<UpdatePeriodicalImageRequest>
{
    private readonly IPeriodicalRepository _periodicalRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public UpdatePeriodicalImageRequestHandler(IPeriodicalRepository PeriodicalRepository, 
        IAmACommandProcessor commandProcessor)
    {
        _periodicalRepository = PeriodicalRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]

    public override async Task<UpdatePeriodicalImageRequest> HandleAsync(UpdatePeriodicalImageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var periodical = await _periodicalRepository.GetPeriodicalById(command.LibraryId, command.PeriodicalId, cancellationToken);

        if (periodical == null)
        {
            throw new NotFoundException();
        }

        var fileName = FilePathHelper.GetPeriodicalImageFileName(command.Image.FileName);
        var filePath = FilePathHelper.GetPeriodicalImageFilePath(command.PeriodicalId, fileName);

        var saveContentCommand = new SaveFileCommand(fileName, filePath, command.Image.Contents)
        {
            MimeType = command.Image.MimeType,
            ExistingFileId = periodical.ImageId,
            IsPublic = true
        };

        await _commandProcessor.SendAsync(saveContentCommand, cancellationToken: cancellationToken);

        command.Result.File = saveContentCommand.Result;

        if (!periodical.ImageId.HasValue)
        {
            await _periodicalRepository.UpdatePeriodicalImage(command.LibraryId, periodical.Id, command.Result.File.Id, cancellationToken);
            command.Result.HasAddedNew = true;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
