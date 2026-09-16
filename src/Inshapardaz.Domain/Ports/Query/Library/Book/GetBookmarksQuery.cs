using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Book;

public class GetBookmarksQuery(int libraryId, int accountId, int bookId) : LibraryBaseQuery<IEnumerable<BookmarkModel>>(libraryId)
{
    public int AccountId { get; } = accountId;
    public int BookId { get; } = bookId;
}

public class GetBookmarksQueryHandler(IBookRepository bookRepository)
    : QueryHandlerAsync<GetBookmarksQuery, IEnumerable<BookmarkModel>>
{
    [LibraryAuthorize(1, Role.Reader, Role.Writer, Role.Admin, Role.LibraryAdmin)]
    public override async Task<IEnumerable<BookmarkModel>> ExecuteAsync(GetBookmarksQuery query, CancellationToken cancellationToken = new CancellationToken()) =>
        await bookRepository.GetBookmarks(query.LibraryId, query.AccountId, query.BookId, cancellationToken);
}
