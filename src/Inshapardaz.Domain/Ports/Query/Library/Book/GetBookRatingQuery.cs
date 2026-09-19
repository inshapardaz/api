using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Book;

public class GetBookRatingQuery(int libraryId, int accountId, int bookId) : LibraryBaseQuery<RatingModel>(libraryId)
{
    public int AccountId { get; } = accountId;
    public int BookId { get; } = bookId;
}

public class GetBookRatingQueryHandler(IBookRepository bookRepository)
    : QueryHandlerAsync<GetBookRatingQuery, RatingModel>
{
    [LibraryAuthorize(1, Role.Reader, Role.Writer, Role.Admin, Role.LibraryAdmin)]
    public override async Task<RatingModel> ExecuteAsync(GetBookRatingQuery query, CancellationToken cancellationToken = new CancellationToken()) =>
        await bookRepository.GetBookRating(query.LibraryId, query.AccountId, query.BookId, cancellationToken);
}
