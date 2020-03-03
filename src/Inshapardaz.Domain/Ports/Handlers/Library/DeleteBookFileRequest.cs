using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeleteBookFileRequest : BookRequest
    {
        public DeleteBookFileRequest(ClaimsPrincipal claims, int libraryId, int bookId, int fileId, Guid userId)
            : base(claims, libraryId, bookId, userId)
        {
            FileId = fileId;
        }

        public int FileId { get; }
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

        [Authorise(step: 1, HandlerTiming.Before)]
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
