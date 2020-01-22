using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using FileModel = Inshapardaz.Domain.Models.FileModel;

namespace Inshapardaz.Domain.Ports.Library
{
    public class UpdateBookImageRequest : RequestBase
    {
        public UpdateBookImageRequest(int bookId)
        {
            BookId = bookId;
        }

        public int BookId { get; }

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
            var book = await _bookRepository.GetBookById(command.BookId, cancellationToken);

            if (book == null)
            {
                throw new NotFoundException();
            }

            if (book.ImageId.HasValue)
            {
                command.Image.Id = book.ImageId.Value;
                var existingImage = await _fileRepository.GetFileById(book.ImageId.Value, true, cancellationToken);
                if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
                {
                    await _fileStorage.TryDeleteFile(existingImage.FilePath, cancellationToken);
                }

                var url = await AddImageToFileStore(book.Id, command.Image.FileName, command.Image.Contents, cancellationToken);

                await _fileRepository.UpdateFile(command.Image, url, true, cancellationToken);
                command.Result.File = command.Image;
                command.Result.File.Id = book.ImageId.Value;
            }
            else
            {
                command.Image.Id = default(int);
                var url = await AddImageToFileStore(book.Id, command.Image.FileName, command.Image.Contents, cancellationToken);
                command.Result.File = await _fileRepository.AddFile(command.Image, url, true, cancellationToken);
                command.Result.HasAddedNew = true;

                book.ImageId = command.Result.File.Id;
                await _bookRepository.UpdateBook(book, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<string> AddImageToFileStore(int bookId, string fileName, byte[] contents, CancellationToken cancellationToken)
        {
            var filePath = GetUniqueFileName(bookId, fileName);
            return await _fileStorage.StoreFile(filePath, contents, cancellationToken);
        }

        private static string GetUniqueFileName(int bookId, string fileName)
        {
            var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
            return $"books/{bookId}/{Guid.NewGuid():N}.{fileNameWithourExtension}";
        }
    }
}
