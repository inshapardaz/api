using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeleteBookFileRequest : RequestBase
    {
        public DeleteBookFileRequest(int bookId, int fileId, System.Guid userId)
        {
            BookId = bookId;
            FileId = fileId;
            UserId = userId;
        }

        public int BookId { get; }

        public int FileId { get;  }
        public Guid UserId { get; }
    }

    public class DeleteBookFileRequestHandler : RequestHandlerAsync<DeleteBookFileRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public DeleteBookFileRequestHandler(IBookRepository bookRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _bookRepository = bookRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<DeleteBookFileRequest> HandleAsync(DeleteBookFileRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var file = await _bookRepository.GetBookFileById(command.BookId, command.FileId, cancellationToken);
            if (file != null)
            {
                await _fileStorage.TryDeleteFile(file.FilePath, cancellationToken);
                await _bookRepository.DeleteBookFile(command.BookId, command.FileId, cancellationToken);
                await _fileRepository.DeleteFile(command.FileId, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
