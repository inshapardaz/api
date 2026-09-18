using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.BookShelf;

public class UpdateBookShelfImageRequest(int libraryId, int bookShelfId) : LibraryBaseCommand(libraryId)
{
    public int BookShelfId { get; } = bookShelfId;
    public FileModel Image { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public FileModel File { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateBookShelfImageRequestHandler(
    IBookShelfRepository bookShelfRepository,
    IFileRepository fileRepository,
    IFileStorage fileStorage,
    IUserHelper userHelper)
    : RequestHandlerAsync<UpdateBookShelfImageRequest>
{
    [LibraryAuthorize(1)]
    public override async Task<UpdateBookShelfImageRequest> HandleAsync(UpdateBookShelfImageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var bookShelf = await bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelfId, cancellationToken);

        if (bookShelf == null)
        {
            throw new NotFoundException();
        }

        if (bookShelf.AccountId != userHelper.AccountId)
        {
            throw new ForbiddenException();
        }

        if (bookShelf.ImageId.HasValue)
        {
            command.Image.Id = bookShelf.ImageId.Value;

            var existingImage = await fileRepository.GetFileById(bookShelf.ImageId.Value, cancellationToken);
            if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
            {
                await fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
            }

            var url = await AddImageToFileStore(bookShelf.Id, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);
            command.Image.FilePath = url;
            command.Image.IsPublic = true;
            command.Image.Checksum = ChecksumHelper.ComputeChecksum(command.Image.Contents);
            await fileRepository.UpdateFile(command.Image, cancellationToken);
            command.Result.File = command.Image;
            command.Result.File.Id = bookShelf.ImageId.Value;
        }
        else
        {
            command.Image.Id = default;
            var url = await AddImageToFileStore(bookShelf.Id, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);
            command.Image.FilePath = url;
            command.Image.IsPublic = true;
            command.Image.Checksum = ChecksumHelper.ComputeChecksum(command.Image.Contents);
            command.Result.File = await fileRepository.AddFile(command.Image, cancellationToken);
            command.Result.HasAddedNew = true;

            await bookShelfRepository.UpdateBookShelfImage(command.LibraryId, command.BookShelfId, command.Result.File.Id, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }

    private async Task<string> AddImageToFileStore(int BookShelfId, string fileName, byte[] contents, string mimeType, CancellationToken cancellationToken)
    {
        var filePath = GetUniqueFileName(BookShelfId, fileName);
        return await fileStorage.StoreImage(filePath, contents, mimeType, cancellationToken);
    }

    private static string GetUniqueFileName(int BookShelfId, string fileName)
    {
        var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
        return $"BookShelf/{BookShelfId}/title.{fileNameWithourExtension}";
    }
}
