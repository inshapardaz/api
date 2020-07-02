using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetCategoryByIdQuery : LibraryBaseQuery<CategoryModel>
    {
        public GetCategoryByIdQuery(int libraryId, int categoryId)
            : base(libraryId)
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
            return await _categoryRepository.GetCategoryById(command.LibraryId, command.CategoryId, cancellationToken);
        }
    }
}
