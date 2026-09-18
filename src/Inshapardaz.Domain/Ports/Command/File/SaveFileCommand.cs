using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Helpers;
using Paramore.Brighter;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Domain.Ports.Command.File;

public class SaveFileCommand(string fileName, string path, byte[] contents) : RequestBase
{
    public string Path { get; set; } = path;
    public string FileName { get; set; } = fileName;

    public byte[] Contents { get; set; } = contents;
    public string MimeType { get; internal set; }

    public bool IsPublic { get; set; }

    public long? ExistingFileId { get; set; }

    public FileModel Result { get; internal set; }
}

public class SaveFileCommandHandler(IFileRepository fileRepository, IFileStorage fileStorage)
    : RequestHandlerAsync<SaveFileCommand>
{
    public override async Task<SaveFileCommand> HandleAsync(SaveFileCommand command, CancellationToken cancellationToken = new CancellationToken())
    {
        var checksum = ChecksumHelper.ComputeChecksum(command.Contents);

        if (command.ExistingFileId.HasValue)
        {
            var file = await fileRepository.GetFileById(command.ExistingFileId.Value, cancellationToken);
            await fileStorage.DeleteFile(file.FilePath, cancellationToken);
            var url = await fileStorage.StoreFile(command.Path, command.Contents, command.MimeType, cancellationToken);
            command.Result = await fileRepository.UpdateFile(new FileModel
            {
                Id = command.ExistingFileId.Value,
                FileName = command.FileName,
                FilePath = command.Path,
                MimeType = command.MimeType,
                IsPublic = command.IsPublic,
                Checksum = checksum
            }, cancellationToken);
        }
        else
        {
            var url = await fileStorage.StoreFile(command.Path, command.Contents, command.MimeType, cancellationToken);
            command.Result = await fileRepository.AddFile(new FileModel
            {
                FileName = command.FileName,
                FilePath = command.Path,
                MimeType = command.MimeType,
                IsPublic = command.IsPublic,
                Checksum = checksum
            }, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
