using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteBookPageImageRequest : LibraryAuthorisedCommand
    {
        public DeleteBookPageImageRequest(ClaimsPrincipal claims, int libraryId, int bookId, int sequenceNumber)
            : base(claims, libraryId)
        {
            BookId = bookId;
            SequenceNumber = sequenceNumber;
        }

        public int BookId { get; }

        public int SequenceNumber { get; }
    }

    public class DeleteBookPageImageRequestHandler : RequestHandlerAsync<DeleteBookPageImageRequest>
    {
        private readonly IBookPageRepository _bookPageRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public DeleteBookPageImageRequestHandler(IBookPageRepository bookPageRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _bookPageRepository = bookPageRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        [Authorise(step: 1, HandlerTiming.Before, Permission.Admin, Permission.LibraryAdmin, Permission.Writer)]
        public override async Task<DeleteBookPageImageRequest> HandleAsync(DeleteBookPageImageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var bookPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);

            if (bookPage != null && bookPage.ImageId.HasValue)
            {
                var existingImage = await _fileRepository.GetFileById(bookPage.ImageId.Value, cancellationToken);
                if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
                {
                    await _fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
                }

                await _fileRepository.DeleteFile(existingImage.Id, cancellationToken);
                await _bookPageRepository.DeletePageImage(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
