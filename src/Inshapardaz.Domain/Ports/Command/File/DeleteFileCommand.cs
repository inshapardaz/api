using Inshapardaz.Domain.Adapters.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.File;

public class DeleteFileCommand(long? fileId) : RequestBase
{
    public long? FileId { get; private set; } = fileId;
}

public class DeleteFileCommandHandler(
    IFileRepository fileRepository,
    IFileStorage fileStorage)
    : RequestHandlerAsync<DeleteFileCommand>
{
    [Authorize(1)]
    public override async Task<DeleteFileCommand> HandleAsync(DeleteFileCommand command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (command.FileId.HasValue)
        {
            var file = await fileRepository.GetFileById(command.FileId.Value, cancellationToken);
            if (file != null)
            {
                await fileStorage.TryDeleteFile(file.FilePath, cancellationToken);
                await fileRepository.DeleteFile(file.Id, cancellationToken);
            }
        }
        return await base.HandleAsync(command, cancellationToken);
    }
}
