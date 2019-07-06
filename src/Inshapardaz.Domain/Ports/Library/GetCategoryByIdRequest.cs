using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetCategoryByIdRequest : RequestBase
    {
        public GetCategoryByIdRequest(int categoryId)
        {
            CategoryId = categoryId;
        }

        public Category Result { get; set; }
        public int CategoryId { get; }
    }

    public class GetCategoryByIdRequestHandler : RequestHandlerAsync<GetCategoryByIdRequest>
    {
        private readonly ICategoryRepository _categoryRepository;
        public readonly IBookRepository _bookRepository;

        public GetCategoryByIdRequestHandler(ICategoryRepository categoryRepository, IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
            _categoryRepository = categoryRepository;
        }

        public override async Task<GetCategoryByIdRequest> HandleAsync(GetCategoryByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var category = await _categoryRepository.GetCategoryById(command.CategoryId, cancellationToken);

            category.BookCount = await _bookRepository.GetBookCountByCategory(command.CategoryId, cancellationToken);
            command.Result = category;

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}

