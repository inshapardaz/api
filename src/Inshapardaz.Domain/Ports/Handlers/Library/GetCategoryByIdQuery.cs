using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetCategoryByIdQuery : IQuery<CategoryModel>
    {
        public GetCategoryByIdQuery(int categoryId)
        {
            CategoryId = categoryId;
        }
        
        public int CategoryId { get; }
    }

    public class GetCategoryByIdQueryHandler : QueryHandlerAsync<GetCategoryByIdQuery, CategoryModel>
    {
        private readonly ICategoryRepository _categoryRepository;
        public readonly IBookRepository _bookRepository;

        public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
            _categoryRepository = categoryRepository;
        }

        public override async Task<CategoryModel> ExecuteAsync(GetCategoryByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var category = await _categoryRepository.GetCategoryById(command.CategoryId, cancellationToken);

            if (category != null)
            {
                category.BookCount = await _bookRepository.GetBookCountByCategory(command.CategoryId, cancellationToken);
            }

            return category;
        }
    }
}

