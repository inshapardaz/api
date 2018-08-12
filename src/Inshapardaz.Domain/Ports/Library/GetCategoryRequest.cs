using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetCategoryRequest : RequestBase
    {
        public IEnumerable<Category> Result { get; set; }
    }

    public class GetCategoryRequestHandler : RequestHandlerAsync<GetCategoryRequest>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoryRequestHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public override async Task<GetCategoryRequest> HandleAsync(GetCategoryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _categoryRepository.GetCategory(cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
