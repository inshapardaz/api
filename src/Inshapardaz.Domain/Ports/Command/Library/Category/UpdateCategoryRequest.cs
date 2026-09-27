using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
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

            if (command.Category.ParentCategoryId.HasValue)
            {
                var parent = await categoryRepository.GetCategoryById(command.LibraryId, command.Category.ParentCategoryId.Value, cancellationToken);
                if (parent == null)
                {
                    throw new BadRequestException("Parent category does not exist in this library.");
                }
            }

            var newCategory = await categoryRepository.AddCategory(command.LibraryId, command.Category, cancellationToken);
            command.Result.HasAddedNew = true;
            command.Result.Category = newCategory;
        }
        else
        {
            if (command.Category.ParentCategoryId.HasValue)
            {
                if (command.Category.ParentCategoryId.Value == command.Category.Id)
                {
                    throw new BadRequestException("A category cannot be its own parent.");
                }

                var parent = await categoryRepository.GetCategoryById(command.LibraryId, command.Category.ParentCategoryId.Value, cancellationToken);
                if (parent == null)
                {
                    throw new BadRequestException("Parent category does not exist in this library.");
                }

                await EnsureNotDescendant(command.LibraryId, command.Category.Id, command.Category.ParentCategoryId.Value, cancellationToken);
            }

            await categoryRepository.UpdateCategory(command.LibraryId, command.Category, cancellationToken);
            command.Result.Category = command.Category;
        }

        return await base.HandleAsync(command, cancellationToken);
    }

    private async Task EnsureNotDescendant(int libraryId, int categoryId, int newParentId, CancellationToken cancellationToken)
    {
        var currentId = (int?)newParentId;
        while (currentId.HasValue)
        {
            if (currentId.Value == categoryId)
            {
                throw new BadRequestException("Cannot set a descendant category as the parent.");
            }

            var current = await categoryRepository.GetCategoryById(libraryId, currentId.Value, cancellationToken);
            currentId = current?.ParentCategoryId;
        }
    }
}
