using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Category;

public class GetCategoryByIdQuery : LibraryBaseQuery<CategoryModel>
{
    public GetCategoryByIdQuery(int libraryId, int categoryId)
        : base(libraryId)
    {
        CategoryId = categoryId;
    }

    public int CategoryId { get; }
}

public class GetCategoryByIdQueryHandler : QueryHandlerAsync<GetCategoryByIdQuery, CategoryModel>
{
    private readonly ICategoryRepository _categoryRepository;
    public readonly IBookRepository _bookRepository;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
        _categoryRepository = categoryRepository;
    }

    [LibraryAuthorize(1)]
    public override async Task<CategoryModel> ExecuteAsync(GetCategoryByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        return await _categoryRepository.GetCategoryById(command.LibraryId, command.CategoryId, cancellationToken);
    }
}
