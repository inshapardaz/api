using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdateBookImageRequest : LibraryBaseCommand
    {
        public UpdateBookImageRequest(int libraryId, int bookId, int? accountId)
            : base(libraryId)
        {
            BookId = bookId;
            AccountId = accountId;
        }

        public int BookId { get; }
        public int? AccountId { get; }
        public FileModel Image { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public FileModel File { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateBookImageRequestHandler : RequestHandlerAsync<UpdateBookImageRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public UpdateBookImageRequestHandler(IBookRepository bookRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _bookRepository = bookRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<UpdateBookImageRequest> HandleAsync(UpdateBookImageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.AccountId, cancellationToken);

            if (book == null)
            {
                throw new NotFoundException();
            }

            if (book.ImageId.HasValue)
            {
                command.Image.Id = book.ImageId.Value;
                var existingImage = await _fileRepository.GetFileById(book.ImageId.Value, cancellationToken);
                if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
                {
                    await _fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
                }

                var url = await AddImageToFileStore(book.Id, command.Image.FileName, command.Image.Contents, cancellationToken);

                command.Image.FilePath = url;
                command.Image.IsPublic = true;
                await _fileRepository.UpdateFile(command.Image, cancellationToken);
                command.Result.File = command.Image;
                command.Result.File.Id = book.ImageId.Value;
            }
            else
            {
                command.Image.Id = default(int);
                var url = await AddImageToFileStore(book.Id, command.Image.FileName, command.Image.Contents, cancellationToken);
                command.Image.FilePath = url;
                command.Image.IsPublic = true;
                command.Result.File = await _fileRepository.AddFile(command.Image, cancellationToken);
                command.Result.HasAddedNew = true;

                await _bookRepository.UpdateBookImage(command.LibraryId, command.BookId, command.Result.File.Id, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<string> AddImageToFileStore(int bookId, string fileName, byte[] contents, CancellationToken cancellationToken)
        {
            var filePath = GetUniqueFileName(bookId, fileName);
            return await _fileStorage.StoreImage(filePath, contents, cancellationToken);
        }

        private static string GetUniqueFileName(int bookId, string fileName)
        {
            var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
            return $"books/{bookId}/{Guid.NewGuid():N}.{fileNameWithourExtension}";
        }
    }
}
