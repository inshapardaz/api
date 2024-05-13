using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Ports.Command;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Category
{
    public class DeleteCategoryRequest : LibraryBaseCommand
    {
        public DeleteCategoryRequest(int libraryId, int categoryId)
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

        [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
        public override async Task<DeleteCategoryRequest> HandleAsync(DeleteCategoryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _categoryRepository.DeleteCategory(command.LibraryId, command.CategoryId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
