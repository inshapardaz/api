using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Category;

public class GetCategoryByIdQuery(int libraryId, int categoryId) : LibraryBaseQuery<CategoryModel>(libraryId)
{
    public int CategoryId { get; } = categoryId;
}

public class GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, IBookRepository bookRepository)
    : QueryHandlerAsync<GetCategoryByIdQuery, CategoryModel>
{
    public readonly IBookRepository _bookRepository = bookRepository;

    [LibraryAuthorize(1)]
    public override async Task<CategoryModel> ExecuteAsync(GetCategoryByIdQuery command, CancellationToken cancellationToken = new CancellationToken()) => await categoryRepository.GetCategoryById(command.LibraryId, command.CategoryId, cancellationToken);
}
