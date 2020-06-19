using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteBookRequest : BookRequest
    {
        public DeleteBookRequest(ClaimsPrincipal claims, int libraryId, int bookId, Guid? userId)
            : base(claims, libraryId, bookId, userId)
        {
        }
    }

    public class DeleteBookRequestHandler : RequestHandlerAsync<DeleteBookRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public DeleteBookRequestHandler(IBookRepository bookRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _bookRepository = bookRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<DeleteBookRequest> HandleAsync(DeleteBookRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.UserId, cancellationToken);
            if (book != null)
            {
                if (book.ImageId.HasValue)
                {
                    var image = await _fileRepository.GetFileById(book.ImageId.Value, cancellationToken);
                    if (image != null && !string.IsNullOrWhiteSpace(image.FilePath))
                    {
                        await _fileStorage.TryDeleteImage(image.FilePath, cancellationToken);
                        await _fileRepository.DeleteFile(image.Id, cancellationToken);
                    }
                }
                await _bookRepository.DeleteBook(command.LibraryId, command.BookId, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
