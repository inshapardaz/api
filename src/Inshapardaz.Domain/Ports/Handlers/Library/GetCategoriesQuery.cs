using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetCategoriesQuery : LibraryBaseQuery<IEnumerable<CategoryModel>>
    {
        public GetCategoriesQuery(int libraryId)
            : base(libraryId)
        {
        }
    }

    public class GetCategoryQueryHandler : QueryHandlerAsync<GetCategoriesQuery, IEnumerable<CategoryModel>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;

        public GetCategoryQueryHandler(ICategoryRepository categoryRepository, IBookRepository bookRepository)
        {
            _categoryRepository = categoryRepository;
            _bookRepository = bookRepository;
        }

        public override async Task<IEnumerable<CategoryModel>> ExecuteAsync(GetCategoriesQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _categoryRepository.GetCategories(command.LibraryId, cancellationToken);
        }
    }
}
