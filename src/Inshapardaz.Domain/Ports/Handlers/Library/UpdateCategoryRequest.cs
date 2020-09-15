using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdateCategoryRequest : LibraryAuthorisedCommand
    {
        public UpdateCategoryRequest(ClaimsPrincipal claims, int libraryId, CategoryModel category)
            : base(claims, libraryId)
        {
            Category = category;
        }

        public CategoryModel Category { get; }

        public UpdateCategoryResult Result { get; } = new UpdateCategoryResult();

        public class UpdateCategoryResult
        {
            public bool HasAddedNew { get; set; }

            public CategoryModel Category { get; set; }
        }
    }

    public class UpdateCategoryRequestHandler : RequestHandlerAsync<UpdateCategoryRequest>
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryRequestHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [Authorise(step: 1, HandlerTiming.Before, Permission.Admin, Permission.LibraryAdmin)]
        public override async Task<UpdateCategoryRequest> HandleAsync(UpdateCategoryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _categoryRepository.GetCategoryById(command.LibraryId, command.Category.Id, cancellationToken);

            if (result == null)
            {
                command.Category.Id = default(int);
                var newCategory = await _categoryRepository.AddCategory(command.LibraryId, command.Category, cancellationToken);
                command.Result.HasAddedNew = true;
                command.Result.Category = newCategory;
            }
            else
            {
                await _categoryRepository.UpdateCategory(command.LibraryId, command.Category, cancellationToken);
                command.Result.Category = command.Category;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
