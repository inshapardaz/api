using Inshapardaz.Domain.Adapters.Repositories;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.File;

public class DeleteFileCommand : RequestBase
{
    public DeleteFileCommand(long? fileId)
    {
        FileId = fileId;
    }

    public long? FileId { get; private set; }
}

public class DeleteFileCommandHandler : RequestHandlerAsync<DeleteFileCommand>
{
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public DeleteFileCommandHandler(IFileRepository fileRepository, 
        IFileStorage fileStorage)
    {
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    [Authorize(1)]
    public override async Task<DeleteFileCommand> HandleAsync(DeleteFileCommand command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (command.FileId.HasValue)
        {
            var file = await _fileRepository.GetFileById(command.FileId.Value, cancellationToken);
            if (file != null)
            {
                await _fileStorage.TryDeleteFile(file.FilePath, cancellationToken);
                await _fileRepository.DeleteFile(file.Id, cancellationToken);
            }
        }
        return await base.HandleAsync(command, cancellationToken);
    }
}
