using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface ICategoryRepository
    {
        Task<Category> AddCategory(Category category, CancellationToken cancellationToken);

        Task UpdateCategory(Category category, CancellationToken cancellationToken);

        Task DeleteCategory(int categoryId, CancellationToken cancellationToken);

        Task<IEnumerable<Category>> GetCategories(CancellationToken cancellationToken);
        
        Task<Category> GetCategoryById(int categoryId, CancellationToken cancellationToken);
    }
}