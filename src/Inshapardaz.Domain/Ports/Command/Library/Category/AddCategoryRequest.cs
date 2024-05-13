using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Category;

public class AddCategoryRequest : LibraryBaseCommand
{
    public AddCategoryRequest(int libraryId, CategoryModel category)
        : base(libraryId)
    {
        Category = category;
    }

    public CategoryModel Category { get; }
    public CategoryModel Result { get; set; }
}

public class AddCategoryRequestHandler : RequestHandlerAsync<AddCategoryRequest>
{
    private readonly ICategoryRepository _categoryRepository;

    public AddCategoryRequestHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [LibraryAuthorize(1, Role.Admin, Role.LibraryAdmin)]
    public override async Task<AddCategoryRequest> HandleAsync(AddCategoryRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        command.Result = await _categoryRepository.AddCategory(command.LibraryId, command.Category, cancellationToken);
        return await base.HandleAsync(command, cancellationToken);
    }
}
