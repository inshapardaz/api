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
    public class AddBookFileRequest : RequestBase
    {
        public AddBookFileRequest(int bookId)
        {
            BookId = bookId;
        }

        public int BookId { get; }

        public File Content { get; set; }


        public File Result { get; set; }
    }

    public class AddBookFileRequestHandler : RequestHandlerAsync<AddBookFileRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public AddBookFileRequestHandler(IBookRepository bookRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _bookRepository = bookRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<AddBookFileRequest> HandleAsync(AddBookFileRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var url = await AddImageToFileStore(command.BookId, command.Content.FileName, command.Content.Contents, cancellationToken);
            var file = await _fileRepository.AddFile(command.Content, url, true, cancellationToken);
            await _bookRepository.AddBookFile(command.BookId, file.Id, cancellationToken);

            command.Result = file;
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
