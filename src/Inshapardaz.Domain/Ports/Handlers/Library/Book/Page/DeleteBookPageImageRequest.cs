using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Book.Page
{
    public class DeleteBookPageImageRequest : LibraryBaseCommand
    {
        public DeleteBookPageImageRequest(int libraryId, int bookId, int sequenceNumber)
            : base(libraryId)
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
