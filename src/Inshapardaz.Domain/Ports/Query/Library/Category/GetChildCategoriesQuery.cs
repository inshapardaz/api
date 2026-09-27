using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Category;

public class GetChildCategoriesQuery(int libraryId, int categoryId) : LibraryBaseQuery<IEnumerable<CategoryModel>>(libraryId)
{
    public int CategoryId { get; } = categoryId;
}

public class GetChildCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : QueryHandlerAsync<GetChildCategoriesQuery, IEnumerable<CategoryModel>>
{
    [LibraryAuthorize(1)]
    public override async Task<IEnumerable<CategoryModel>> ExecuteAsync(GetChildCategoriesQuery command, CancellationToken cancellationToken = new CancellationToken()) =>
        await categoryRepository.GetChildCategories(command.LibraryId, command.CategoryId, cancellationToken);
}
