using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface ICategoryRepository
    {
        Task<CategoryModel> AddCategory(int libraryId, CategoryModel category, CancellationToken cancellationToken);

        Task UpdateCategory(int libraryId, CategoryModel category, CancellationToken cancellationToken);

        Task DeleteCategory(int libraryId, int categoryId, CancellationToken cancellationToken);

        Task<IEnumerable<CategoryModel>> GetCategories(int libraryId, CancellationToken cancellationToken);

        Task<CategoryModel> GetCategoryById(int libraryId, int categoryId, CancellationToken cancellationToken);
    }
}
