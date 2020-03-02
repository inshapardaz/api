using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Dictionaries;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddCategoryRequest : LibraryAuthorisedCommand
    {
        public AddCategoryRequest(ClaimsPrincipal claims, int libraryId, CategoryModel category)
            : base(claims, libraryId)
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

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<AddCategoryRequest> HandleAsync(AddCategoryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _categoryRepository.AddCategory(command.LibraryId, command.Category, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
