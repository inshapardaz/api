using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.File;

public class AddFileRequest : RequestBase
{
    public AddFileRequest(FileModel file)
    {
        File = file;
    }

    public FileModel File { get; set; }
    public FileModel Response { get; set; }
}

public class AddFileRequestHandler : RequestHandlerAsync<AddFileRequest>
{
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public AddFileRequestHandler(IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    public override async Task<AddFileRequest> HandleAsync(AddFileRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var url = await AddImageToFileStore(command.File.FileName, command.File.Contents, cancellationToken);
        command.File.FilePath = url;
        command.File.IsPublic = true;
        command.Response = await _fileRepository.AddFile(command.File, cancellationToken);
        return await base.HandleAsync(command, cancellationToken);
    }

    private async Task<string> AddImageToFileStore(string fileName, byte[] contents, CancellationToken cancellationToken)
    {
        var filePath = GetUniqueFileName(fileName);
        return await _fileStorage.StoreFile(filePath, contents, cancellationToken);
    }

    private static string GetUniqueFileName(string fileName)
    {
        var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
        return $"images/{Guid.NewGuid():N}.{fileNameWithourExtension}";
    }
}
