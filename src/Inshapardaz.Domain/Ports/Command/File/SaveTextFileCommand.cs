using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Helpers;
using Paramore.Brighter;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Domain.Ports.Command.File;

public class SaveTextFileCommand(string fileName, string path, string contents) : RequestBase
{
    public string Path { get; set; } = path;
    public string FileName { get; set; } = fileName;

    public string Contents { get; set; } = contents;
    public string MimeType { get; internal set; }

    public bool IsPublic { get; set; }

    public long? ExistingFileId { get; set; }

    public FileModel Result { get; internal set; }
}

public class SaveTextFileCommandHandler(IFileRepository fileRepository, IFileStorage fileStorage)
    : RequestHandlerAsync<SaveTextFileCommand>
{
    public override async Task<SaveTextFileCommand> HandleAsync(SaveTextFileCommand command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (command.ExistingFileId.HasValue)
        {
            var file = await fileRepository.GetFileById(command.ExistingFileId.Value, cancellationToken);
            var url = await fileStorage.StoreTextFile(file.FilePath, command.Contents, cancellationToken);
            command.Result = file;
        }
        else 
        {
            var url = await fileStorage.StoreTextFile(command.Path, command.Contents, cancellationToken);
            command.Result = await fileRepository.AddFile(new FileModel
            {
                FileName = command.FileName,
                FilePath = command.Path,
                MimeType = command.MimeType,
                DateCreated = DateTime.Now,
                IsPublic = command.IsPublic,
                Checksum = ChecksumHelper.Compute(command.Contents)
            }, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
