using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
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
            return await _categoryRepository.GetCategoryById(command.Libraryid, command.CategoryId, cancellationToken);
        }
    }
}
