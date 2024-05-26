using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Category;

public class UpdateCategoryRequest : LibraryBaseCommand
{
    public UpdateCategoryRequest(int libraryId, CategoryModel category)
        : base(libraryId)
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

    [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
    public override async Task<UpdateCategoryRequest> HandleAsync(UpdateCategoryRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await _categoryRepository.GetCategoryById(command.LibraryId, command.Category.Id, cancellationToken);

        if (result == null)
        {
            command.Category.Id = default;
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
