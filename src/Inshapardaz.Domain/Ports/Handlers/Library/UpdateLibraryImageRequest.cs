using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library
{
    public class UpdateLibraryImageRequest : LibraryBaseCommand
    {
        public UpdateLibraryImageRequest(int libraryId)
            : base(libraryId)
        {
        }

        public FileModel Image { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public FileModel File { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateLibraryImageRequestHandler : RequestHandlerAsync<UpdateLibraryImageRequest>
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public UpdateLibraryImageRequestHandler(ILibraryRepository libraryRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _libraryRepository = libraryRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<UpdateLibraryImageRequest> HandleAsync(UpdateLibraryImageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);

            if (library == null)
            {
                throw new NotFoundException();
            }

            if (library.ImageId.HasValue)
            {
                command.Image.Id = library.ImageId.Value;

                var existingImage = await _fileRepository.GetFileById(library.ImageId.Value, cancellationToken);
                if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
                {
                    await _fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
                }

                var url = await AddImageToFileStore(library.Id, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);
                command.Image.FilePath = url;
                command.Image.IsPublic = true;
                await _fileRepository.UpdateFile(command.Image, cancellationToken);
                command.Result.File = command.Image;
                command.Result.File.Id = library.ImageId.Value;
            }
            else
            {
                command.Image.Id = default;
                var url = await AddImageToFileStore(library.Id, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);
                command.Image.FilePath = url;
                command.Image.IsPublic = true;
                command.Result.File = await _fileRepository.AddFile(command.Image, cancellationToken);
                command.Result.HasAddedNew = true;

                await _libraryRepository.UpdateLibraryImage(command.LibraryId, command.Result.File.Id, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<string> AddImageToFileStore(int libraryId, string fileName, byte[] contents, string mimeType, CancellationToken cancellationToken)
        {
            var filePath = GetUniqueFileName(libraryId, fileName);
            return await _fileStorage.StoreImage(filePath, contents, mimeType, cancellationToken);
        }

        private static string GetUniqueFileName(int libraryId, string fileName)
        {
            var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
            return $"libraries/{libraryId}/image.{fileNameWithourExtension}";
        }
    }
}
