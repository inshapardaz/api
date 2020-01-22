using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface ICategoryRepository
    {
        Task<CategoryModel> AddCategory(CategoryModel category, CancellationToken cancellationToken);

        Task UpdateCategory(CategoryModel category, CancellationToken cancellationToken);

        Task DeleteCategory(int categoryId, CancellationToken cancellationToken);

        Task<IEnumerable<CategoryModel>> GetCategories(CancellationToken cancellationToken);
        
        Task<CategoryModel> GetCategoryById(int categoryId, CancellationToken cancellationToken);
    }
}