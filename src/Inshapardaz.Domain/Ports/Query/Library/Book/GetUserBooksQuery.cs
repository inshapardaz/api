using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Book;

public class GetUserBooksQuery(int libraryId, int accountId, int pageNumber, int pageSize)
    : LibraryBaseQuery<Page<BookModel>>(libraryId)
{
    public int PageNumber { get; private set; } = pageNumber;

    public int PageSize { get; private set; } = pageSize;
    public StatusType StatusFilter { get; set; }
    public int AccountId { get; set; } = accountId;
}

public class GetUserBooksQueryHandler(IBookRepository bookRepository)
    : QueryHandlerAsync<GetUserBooksQuery, Page<BookModel>>
{
    public override async Task<Page<BookModel>> ExecuteAsync(GetUserBooksQuery query, CancellationToken cancellationToken = new CancellationToken()) => await bookRepository.GetBooksByUser(query.LibraryId, query.AccountId, query.PageNumber, query.PageSize, query.StatusFilter, BookSortByType.Title, SortDirection.Ascending, cancellationToken);
}
