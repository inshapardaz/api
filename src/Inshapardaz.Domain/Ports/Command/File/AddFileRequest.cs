using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.File;

public class AddFileRequest(FileModel file) : RequestBase
{
    public FileModel File { get; set; } = file;
    public FileModel Response { get; set; }
}

public class AddFileRequestHandler(IFileRepository fileRepository, IFileStorage fileStorage)
    : RequestHandlerAsync<AddFileRequest>
{
    public override async Task<AddFileRequest> HandleAsync(AddFileRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var url = await AddImageToFileStore(command.File.FileName, command.File.Contents, command.File.MimeType, cancellationToken);
        command.File.FilePath = url;
        command.File.IsPublic = true;
        command.Response = await fileRepository.AddFile(command.File, cancellationToken);
        return await base.HandleAsync(command, cancellationToken);
    }

    private async Task<string> AddImageToFileStore(string fileName, byte[] contents, string mimeType, CancellationToken cancellationToken)
    {
        var filePath = GetUniqueFileName(fileName);
        return await fileStorage.StoreFile(filePath, contents, mimeType, cancellationToken);
    }

    private static string GetUniqueFileName(string fileName)
    {
        var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
        return $"images/{Guid.NewGuid():N}.{fileNameWithourExtension}";
    }
}
