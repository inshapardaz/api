using System.Threading;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;

namespace Inshapardaz.Api.IntegrationTests.DataHelper
{
    public class CategoryDataHelper
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryDataHelper(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Category Create(string category)
        {
            return _categoryRepository.AddCategory(new Category { Name = category} , CancellationToken.None).Result;
        }

        public Category Get(int categoryId)
        {
            return _categoryRepository.GetCategoryById(categoryId, CancellationToken.None).Result;
        }

        public void Delete(int categoryId)
        {
            _categoryRepository.DeleteCategory(categoryId, CancellationToken.None).Wait();
        }
    }
}