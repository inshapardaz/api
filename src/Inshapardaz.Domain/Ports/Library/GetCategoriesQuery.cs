using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetCategoriesQuery : IQuery<IEnumerable<Category>>
    {
    }

    public class GetCategoryQueryHandler : QueryHandlerAsync<GetCategoriesQuery, IEnumerable<Category>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;

        public GetCategoryQueryHandler(ICategoryRepository categoryRepository, IBookRepository bookRepository)
        {
            _categoryRepository = categoryRepository;
            _bookRepository = bookRepository;
        }

        public override async Task<IEnumerable<Category>> ExecuteAsync(GetCategoriesQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var categories = await _categoryRepository.GetCategories(cancellationToken);

            foreach (var category in categories)
            {
                category.BookCount = await _bookRepository.GetBookCountByCategory(category.Id, cancellationToken);
            }

            return categories;
        }
    }
}
