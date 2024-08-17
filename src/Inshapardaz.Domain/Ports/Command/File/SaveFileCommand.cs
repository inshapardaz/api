using Inshapardaz.Domain.Adapters.Repositories;
using Paramore.Brighter;
using System.Threading.Tasks;
using System.Threading;
using System;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Domain.Ports.Command.File;

public class SaveFileCommand: RequestBase
{
    public SaveFileCommand(string fileName, string path, byte[] contents)
    {
        Path = path;
        FileName = fileName;
        Contents = contents;
    }

    public string Path { get; set; }
    public string FileName { get; set; }

    public byte[] Contents { get; set; }
    public string MimeType { get; internal set; }

    public bool IsPublic { get; set; }

    public long? ExistingFileId { get; set; }

    public FileModel Result { get; internal set; }
}

public class SaveFileCommandHandler : RequestHandlerAsync<SaveFileCommand>
{
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public SaveFileCommandHandler(IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    public override async Task<SaveFileCommand> HandleAsync(SaveFileCommand command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (command.ExistingFileId.HasValue)
        {
            var file = await _fileRepository.GetFileById(command.ExistingFileId.Value, cancellationToken);
            await _fileStorage.DeleteFile(file.FilePath, cancellationToken);
            var url = await _fileStorage.StoreFile(command.Path, command.Contents, command.MimeType, cancellationToken);
            command.Result = await _fileRepository.UpdateFile(new FileModel
            {
                Id = command.ExistingFileId.Value,
                FileName = command.FileName,
                FilePath = command.Path,
                MimeType = command.MimeType,
                IsPublic = command.IsPublic
            }, cancellationToken);
        }
        else 
        {
            var url = await _fileStorage.StoreFile(command.Path, command.Contents, command.MimeType, cancellationToken);
            command.Result = await _fileRepository.AddFile(new FileModel
            {
                FileName = command.FileName,
                FilePath = command.Path,
                MimeType = command.MimeType,
                IsPublic = command.IsPublic
            }, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
