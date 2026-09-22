using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Category;

public class GetCategoryTreeQuery(int libraryId) : LibraryBaseQuery<IEnumerable<CategoryModel>>(libraryId);

public class GetCategoryTreeQueryHandler(ICategoryRepository categoryRepository)
    : QueryHandlerAsync<GetCategoryTreeQuery, IEnumerable<CategoryModel>>
{
    [LibraryAuthorize(1)]
    public override async Task<IEnumerable<CategoryModel>> ExecuteAsync(GetCategoryTreeQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var categories = (await categoryRepository.GetCategories(command.LibraryId, cancellationToken)).ToList();
        var lookup = categories.ToDictionary(c => c.Id);

        foreach (var category in categories)
        {
            category.Children = new List<CategoryModel>();
        }

        foreach (var category in categories)
        {
            if (category.ParentCategoryId.HasValue && lookup.TryGetValue(category.ParentCategoryId.Value, out var parent))
            {
                parent.Children.Add(category);
            }
        }

        return categories.Where(c => !c.ParentCategoryId.HasValue);
    }
}
