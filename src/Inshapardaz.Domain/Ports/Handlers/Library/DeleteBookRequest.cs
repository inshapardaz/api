using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteBookRequest : BookRequest
    {
        public DeleteBookRequest(int libraryId, int bookId, int? accountId)
            : base(libraryId, bookId)
        {
            AccountId = accountId;
        }

        public int? AccountId { get; }
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
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.AccountId, cancellationToken);
            if (book != null)
            {
                var status = (await _bookRepository.GetBookPageSummary(command.LibraryId, new[] { book.Id }, cancellationToken)).FirstOrDefault();

                if (status != null)
                {
                    book.PageStatus = status.Statuses;
                    if (status.Statuses.Any(s => s.Status == PageStatuses.Completed))
                    {
                        decimal completedPages = (decimal)status.Statuses.Single(s => s.Status == PageStatuses.Completed).Count;
                        decimal totalPages = (decimal)status.Statuses.Sum(s => s.Count);
                        book.Progress = (completedPages / totalPages) * 100;
                    }
                    else
                    {
                        book.Progress = 0.0M;
                    }
                }

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
