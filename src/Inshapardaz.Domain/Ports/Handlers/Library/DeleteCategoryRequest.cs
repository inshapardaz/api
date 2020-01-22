using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeleteCategoryRequest : RequestBase
    {
        public DeleteCategoryRequest(int categoryId)
        {
            CategoryId = categoryId;
        }

        public int CategoryId { get; }
    }

    public class DeleteCategoryRequestHandler : RequestHandlerAsync<DeleteCategoryRequest>
    {
        private readonly ICategoryRepository _categoryRepository;

        public DeleteCategoryRequestHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public override async Task<DeleteCategoryRequest> HandleAsync(DeleteCategoryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _categoryRepository.DeleteCategory(command.CategoryId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
