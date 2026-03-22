using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Book;

public class GetBookByIdQuery(int libraryId, int bookId, int? accountId) : LibraryBaseQuery<BookModel>(libraryId)
{
    public int BookId { get; private set; } = bookId;
    public int? AccountId { get; } = accountId;
}

public class GetBookByIdQueryHandler(IBookRepository bookRepository, IFileRepository fileRepository)
    : QueryHandlerAsync<GetBookByIdQuery, BookModel>
{
    public override async Task<BookModel> ExecuteAsync(GetBookByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, command.AccountId, cancellationToken);
        if (book != null)
        {
            var status = (await bookRepository.GetBookPageSummary(command.LibraryId, new[] { book.Id }, cancellationToken)).FirstOrDefault();

            if (status != null)
            {
                book.PageStatus = status.Statuses;
                if (status.Statuses.Any(s => s.Status == EditingStatus.Completed))
                {
                    decimal completedPages = status.Statuses.Single(s => s.Status == EditingStatus.Completed).Count;
                    book.Progress = completedPages / book.PageCount;
                }
                else
                {
                    book.Progress = 0.0M;
                }
            }

            if (book.ImageUrl == null && book.ImageId.HasValue)
            {
                book.ImageUrl = await ImageHelper.TryConvertToPublicFile(book.ImageId.Value, fileRepository, cancellationToken);
            }

            var contents = await bookRepository.GetBookContents(command.LibraryId, command.BookId, cancellationToken);

            book.Contents = contents.ToList();
        }

        return book;
    }
}
