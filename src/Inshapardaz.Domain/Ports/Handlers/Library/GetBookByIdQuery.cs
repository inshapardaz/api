using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetBookByIdQuery : LibraryBaseQuery<BookModel>
    {
        public GetBookByIdQuery(int libraryId, int bookId, int? accountId)
            : base(libraryId)
        {
            BookId = bookId;
            AccountId = accountId;
        }

        public int BookId { get; private set; }
        public int? AccountId { get; }
    }

    public class GetBookByIdQueryHandler : QueryHandlerAsync<GetBookByIdQuery, BookModel>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFileRepository _fileRepository;

        public GetBookByIdQueryHandler(IBookRepository bookRepository, IFileRepository fileRepository)
        {
            _bookRepository = bookRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<BookModel> ExecuteAsync(GetBookByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
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
                    book.ImageUrl = await ImageHelper.TryConvertToPublicFile(book.ImageId.Value, _fileRepository, cancellationToken);
                }

                var contents = await _bookRepository.GetBookContents(command.LibraryId, command.BookId, cancellationToken);

                book.Contents = contents.ToList();
            }

            return book;
        }
    }
}
