using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeleteBookRequest : RequestBase
    {
        public DeleteBookRequest(int bookId)
        {
            BookId = bookId;
        }

        public int BookId { get; }
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

        public override async Task<DeleteBookRequest> HandleAsync(DeleteBookRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var author = await _bookRepository.GetBookById(command.BookId, cancellationToken);
            if (author != null)
            {
                if (author.ImageId.HasValue)
                {
                    var image = await _fileRepository.GetFileById(author.ImageId.Value, true, cancellationToken);
                    if (image != null && !string.IsNullOrWhiteSpace(image.FilePath))
                    {
                        await _fileStorage.TryDeleteFile(image.FilePath, cancellationToken);
                        await _fileRepository.DeleteFile(image.Id, cancellationToken);
                    }
                }
                await _bookRepository.DeleteBook(command.BookId, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
