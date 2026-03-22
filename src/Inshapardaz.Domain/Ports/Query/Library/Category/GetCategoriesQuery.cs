using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Category;

public class GetCategoriesQuery(int libraryId) : LibraryBaseQuery<IEnumerable<CategoryModel>>(libraryId);

public class GetCategoryQueryHandler(ICategoryRepository categoryRepository)
    : QueryHandlerAsync<GetCategoriesQuery, IEnumerable<CategoryModel>>
{
    [LibraryAuthorize(1)]
    public override async Task<IEnumerable<CategoryModel>> ExecuteAsync(GetCategoriesQuery command, CancellationToken cancellationToken = new CancellationToken()) => await categoryRepository.GetCategories(command.LibraryId, cancellationToken);
}
