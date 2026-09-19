using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Book;

// No [LibraryAuthorize] - the average rating is part of the book's public summary, same
// visibility as the book itself, enforced the same way GetBookByIdQuery is: by passing accountId
// through to the repository and treating a null (inaccessible/non-existing) book as not found.
public class GetBookRatingSummaryQuery(int libraryId, int bookId, int? accountId) : LibraryBaseQuery<RatingSummaryModel>(libraryId)
{
    public int BookId { get; } = bookId;
    public int? AccountId { get; } = accountId;
}

public class GetBookRatingSummaryQueryHandler(IBookRepository bookRepository)
    : QueryHandlerAsync<GetBookRatingSummaryQuery, RatingSummaryModel>
{
    public override async Task<RatingSummaryModel> ExecuteAsync(GetBookRatingSummaryQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(query.LibraryId, query.BookId, query.AccountId, cancellationToken);

        return book == null
            ? null
            : await bookRepository.GetBookRatingSummary(query.LibraryId, query.BookId, cancellationToken);
    }
}
