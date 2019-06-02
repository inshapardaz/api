using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using File = Inshapardaz.Domain.Entities.File;

namespace Inshapardaz.Domain.Ports.Library
{
    public class UpdateBookFileRequest : RequestBase
    {
        public UpdateBookFileRequest(int bookId, int fileId)
        {
            BookId = bookId;
            FileId = fileId;
        }

        public int BookId { get; }

        public int FileId { get; set; }

        public File Content { get; set; }


        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public File Content { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateBookFileRequestHandler : RequestHandlerAsync<UpdateBookFileRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public UpdateBookFileRequestHandler(IBookRepository bookRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _bookRepository = bookRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<UpdateBookFileRequest> HandleAsync(UpdateBookFileRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var bookFile = await _bookRepository.GetBookFileById(command.BookId, command.FileId, cancellationToken);

            if (bookFile != null)
            {
                await _fileStorage.TryDeleteFile(bookFile.FilePath, cancellationToken);

                var url = await AddImageToFileStore(bookFile.Id, command.Content.FileName, command.Content.Contents, cancellationToken);

                await _fileRepository.UpdateFile(command.Content, url, true, cancellationToken);
            }
            else
            {
                var url = await AddImageToFileStore(command.BookId, command.Content.FileName, command.Content.Contents, cancellationToken);
                var file = await _fileRepository.AddFile(command.Content, url, true, cancellationToken);
                await _bookRepository.AddBookFile(command.BookId, file.Id, cancellationToken);

                command.Result.HasAddedNew = true;
                command.Result.Content = file;
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
            var fileNameWithoutExtension = Path.GetExtension(fileName).Trim('.');
            return $"books/{bookId}/{Guid.NewGuid():N}.{fileNameWithoutExtension}";
        }
    }
}
