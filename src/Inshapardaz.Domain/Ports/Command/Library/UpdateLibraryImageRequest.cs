using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library;

public class UpdateLibraryImageRequest(int libraryId) : LibraryBaseCommand(libraryId)
{
    public FileModel Image { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public FileModel File { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateLibraryImageRequestHandler(
    ILibraryRepository libraryRepository,
    IFileRepository fileRepository,
    IFileStorage fileStorage)
    : RequestHandlerAsync<UpdateLibraryImageRequest>
{
    [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
    public override async Task<UpdateLibraryImageRequest> HandleAsync(UpdateLibraryImageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var library = await libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);

        if (library == null)
        {
            throw new NotFoundException();
        }

        if (library.ImageId.HasValue)
        {
            command.Image.Id = library.ImageId.Value;

            var existingImage = await fileRepository.GetFileById(library.ImageId.Value, cancellationToken);
            if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
            {
                await fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
            }

            var url = await AddImageToFileStore(library.Id, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);
            command.Image.FilePath = url;
            command.Image.IsPublic = true;
            command.Image.Checksum = ChecksumHelper.ComputeChecksum(command.Image.Contents);
            await fileRepository.UpdateFile(command.Image, cancellationToken);
            command.Result.File = command.Image;
            command.Result.File.Id = library.ImageId.Value;
        }
        else
        {
            command.Image.Id = default;
            var url = await AddImageToFileStore(library.Id, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);
            command.Image.FilePath = url;
            command.Image.IsPublic = true;
            command.Image.Checksum = ChecksumHelper.ComputeChecksum(command.Image.Contents);
            command.Result.File = await fileRepository.AddFile(command.Image, cancellationToken);
            command.Result.HasAddedNew = true;

            await libraryRepository.UpdateLibraryImage(command.LibraryId, command.Result.File.Id, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }

    private async Task<string> AddImageToFileStore(int libraryId, string fileName, byte[] contents, string mimeType, CancellationToken cancellationToken)
    {
        var filePath = GetUniqueFileName(libraryId, fileName);
        return await fileStorage.StoreImage(filePath, contents, mimeType, cancellationToken);
    }

    private static string GetUniqueFileName(int libraryId, string fileName)
    {
        var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
        return $"libraries/{libraryId}/image.{fileNameWithourExtension}";
    }
}
