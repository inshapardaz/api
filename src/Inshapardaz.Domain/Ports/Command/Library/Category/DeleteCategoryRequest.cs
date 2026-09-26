using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Category;

public class DeleteCategoryRequest(int libraryId, int categoryId) : LibraryBaseCommand(libraryId)
{
    public int CategoryId { get; } = categoryId;
}

public class DeleteCategoryRequestHandler(ICategoryRepository categoryRepository)
    : RequestHandlerAsync<DeleteCategoryRequest>
{
    [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
    public override async Task<DeleteCategoryRequest> HandleAsync(DeleteCategoryRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var children = await categoryRepository.GetChildCategories(command.LibraryId, command.CategoryId, cancellationToken);
        if (children.Any())
        {
            throw new BadRequestException("Cannot delete a category that has child categories. Move or delete the child categories first.");
        }

        await categoryRepository.DeleteCategory(command.LibraryId, command.CategoryId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
