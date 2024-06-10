using Inshapardaz.Domain.Adapters.Repositories;
using Paramore.Brighter;
using System.Threading.Tasks;
using System.Threading;
using System;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Domain.Ports.Command.File;

public class SaveTextFileCommand: RequestBase
{
    public SaveTextFileCommand(string fileName, string path, string contents)
    {
        Path = path;
        FileName = fileName;
        Contents = contents;
    }

    public string Path { get; set; }
    public string FileName { get; set; }

    public string Contents { get; set; }
    public string MimeType { get; internal set; }

    public bool IsPublic { get; set; }

    public long? ExistingFileId { get; set; }

    public FileModel Result { get; internal set; }
}

public class SaveTextFileCommandHandler : RequestHandlerAsync<SaveTextFileCommand>
{
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public SaveTextFileCommandHandler(IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    public override async Task<SaveTextFileCommand> HandleAsync(SaveTextFileCommand command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (command.ExistingFileId.HasValue)
        {
            var file = await _fileRepository.GetFileById(command.ExistingFileId.Value, cancellationToken);
            var url = await _fileStorage.StoreTextFile(file.FilePath, command.Contents, cancellationToken);
            command.Result = file;
        }
        else 
        {
            var url = await _fileStorage.StoreTextFile(command.Path, command.Contents, cancellationToken);
            command.Result = await _fileRepository.AddFile(new FileModel
            {
                FileName = command.FileName,
                FilePath = command.Path,
                MimeType = command.MimeType,
                DateCreated = DateTime.Now,
                IsPublic = command.IsPublic
            }, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
