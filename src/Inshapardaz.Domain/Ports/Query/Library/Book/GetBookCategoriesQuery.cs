using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Book;

public class GetBookCategoriesQuery(int libraryId, int bookId) : LibraryBaseQuery<IEnumerable<CategoryModel>>(libraryId)
{
    public int BookId { get; } = bookId;
}

public class GetBookCategoriesQueryHandler(IBookRepository bookRepository)
    : QueryHandlerAsync<GetBookCategoriesQuery, IEnumerable<CategoryModel>>
{
    [LibraryAuthorize(1)]
    public override async Task<IEnumerable<CategoryModel>> ExecuteAsync(GetBookCategoriesQuery query, CancellationToken cancellationToken = new CancellationToken()) =>
        await bookRepository.GetBookCategories(query.LibraryId, query.BookId, cancellationToken);
}
