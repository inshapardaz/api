using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Book;

public class GetBooksQuery(int libraryId, int pageNumber, int pageSize, int? accountId)
    : LibraryBaseQuery<Page<BookModel>>(libraryId)
{
    public int PageNumber { get; private set; } = pageNumber;

    public int PageSize { get; private set; } = pageSize;
    public int? AccountId { get; } = accountId;
    public string Query { get; set; }

    public BookSortByType SortBy { get; set; }

    public BookFilter Filter { get; set; }
    public SortDirection SortDirection { get; set; }
}

public class GetBooksQueryHandler(IBookRepository bookRepository, IFileRepository fileRepository)
    : QueryHandlerAsync<GetBooksQuery, Page<BookModel>>
{
    public override async Task<Page<BookModel>> ExecuteAsync(GetBooksQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var books = string.IsNullOrWhiteSpace(command.Query)
         ? await bookRepository.GetBooks(command.LibraryId, command.PageNumber, command.PageSize, command.AccountId, command.Filter, command.SortBy, command.SortDirection, cancellationToken)
         : await bookRepository.SearchBooks(command.LibraryId, command.Query, command.PageNumber, command.PageSize, command.AccountId, command.Filter, command.SortBy, command.SortDirection, cancellationToken);

        var statuses = await bookRepository.GetBookPageSummary(command.LibraryId, books.Data.Select(b => b.Id).ToList(), cancellationToken);

        foreach (var status in statuses)
        {
            var book = books.Data.SingleOrDefault(b => b.Id == status.BookId);
            if (book != null)
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
        }

        foreach (var book in books.Data)
        {
            if (book != null && book.ImageUrl == null && book.ImageId.HasValue)
            {
                book.ImageUrl = await ImageHelper.TryConvertToPublicFile(book.ImageId.Value, fileRepository, cancellationToken);
            }

            var contents = await bookRepository.GetBookContents(command.LibraryId, book.Id, cancellationToken);

            book.Contents = contents.ToList();
        }

        return books;
    }
}
