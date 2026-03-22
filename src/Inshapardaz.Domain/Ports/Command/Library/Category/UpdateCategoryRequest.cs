using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Category;

public class UpdateCategoryRequest(int libraryId, CategoryModel category) : LibraryBaseCommand(libraryId)
{
    public CategoryModel Category { get; } = category;

    public UpdateCategoryResult Result { get; } = new UpdateCategoryResult();

    public class UpdateCategoryResult
    {
        public bool HasAddedNew { get; set; }

        public CategoryModel Category { get; set; }
    }
}

public class UpdateCategoryRequestHandler(ICategoryRepository categoryRepository)
    : RequestHandlerAsync<UpdateCategoryRequest>
{
    [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
    public override async Task<UpdateCategoryRequest> HandleAsync(UpdateCategoryRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await categoryRepository.GetCategoryById(command.LibraryId, command.Category.Id, cancellationToken);

        if (result == null)
        {
            command.Category.Id = default;
            var newCategory = await categoryRepository.AddCategory(command.LibraryId, command.Category, cancellationToken);
            command.Result.HasAddedNew = true;
            command.Result.Category = newCategory;
        }
        else
        {
            await categoryRepository.UpdateCategory(command.LibraryId, command.Category, cancellationToken);
            command.Result.Category = command.Category;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
