using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeleteCategoryRequest : LibraryBaseCommand
    {
        public DeleteCategoryRequest(ClaimsPrincipal claims, int libraryId, int categoryId)
            : base(libraryId)
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

        [LibraryAdminAuthorise(step: 1, HandlerTiming.Before)]
        public override async Task<DeleteCategoryRequest> HandleAsync(DeleteCategoryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _categoryRepository.DeleteCategory(command.LibraryId, command.CategoryId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
