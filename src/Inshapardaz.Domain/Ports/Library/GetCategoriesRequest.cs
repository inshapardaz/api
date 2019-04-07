using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetCategoriesRequest : RequestBase
    {
        public IEnumerable<Category> Result { get; set; }
    }

    public class GetCategoryRequestHandler : RequestHandlerAsync<GetCategoriesRequest>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;

        public GetCategoryRequestHandler(ICategoryRepository categoryRepository, IBookRepository bookRepository)
        {
            _categoryRepository = categoryRepository;
            _bookRepository = bookRepository;
        }

        public override async Task<GetCategoriesRequest> HandleAsync(GetCategoriesRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _categoryRepository.GetCategory(cancellationToken);

            foreach (var category in command.Result)
            {
                category.BookCount = await _bookRepository.GetBookCountByCategory(category.Id, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
