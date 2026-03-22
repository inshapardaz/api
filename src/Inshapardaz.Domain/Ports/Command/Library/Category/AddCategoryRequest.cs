using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Category;

public class AddCategoryRequest(int libraryId, CategoryModel category) : LibraryBaseCommand(libraryId)
{
    public CategoryModel Category { get; } = category;
    public CategoryModel Result { get; set; }
}

public class AddCategoryRequestHandler(ICategoryRepository categoryRepository) : RequestHandlerAsync<AddCategoryRequest>
{
    [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
    public override async Task<AddCategoryRequest> HandleAsync(AddCategoryRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        command.Result = await categoryRepository.AddCategory(command.LibraryId, command.Category, cancellationToken);
        return await base.HandleAsync(command, cancellationToken);
    }
}
