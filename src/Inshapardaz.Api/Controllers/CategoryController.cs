using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderCategory _categoryRenderer;

        public CategoryController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderCategory categoryRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _categoryRenderer = categoryRenderer;
        }

        [HttpGet("libraries/{libraryId}/categories", Name = nameof(CategoryController.GetCategories))]
        public async Task<IActionResult> GetCategories(int libraryId, CancellationToken token = default(CancellationToken))
        {
            var query = new GetCategoriesQuery(libraryId);
            var categories = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            return new OkObjectResult(_categoryRenderer.Render(categories, libraryId));
        }

        [HttpGet("libraries/{libraryId}/categories/{categoryId}", Name = nameof(CategoryController.GetCategoryById))]
        public async Task<IActionResult> GetCategoryById(int libraryId, int categoryId, CancellationToken token = default(CancellationToken))
        {
            var query = new GetCategoryByIdQuery(libraryId, categoryId);
            var category = await _queryProcessor.ExecuteAsync(query, token);

            if (category == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(_categoryRenderer.Render(category, libraryId));
        }

        [HttpPost("libraries/{libraryId}/categories", Name = nameof(CategoryController.CreateCategory))]
        [Authorize(Role.Admin, Role.LibraryAdmin)]
        public async Task<IActionResult> CreateCategory(int libraryId, [FromBody]CategoryView category, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AddCategoryRequest(libraryId, category.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _categoryRenderer.Render(request.Result, libraryId);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        [HttpPut("libraries/{libraryId}/categories/{categoryId}", Name = nameof(CategoryController.UpdateCategory))]
        [Authorize(Role.Admin, Role.LibraryAdmin)]
        public async Task<IActionResult> UpdateCategory(int libraryId, int categoryId, [FromBody]CategoryView category, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            category.Id = categoryId;
            var request = new UpdateCategoryRequest(libraryId, category.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _categoryRenderer.Render(request.Result.Category, libraryId);

            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }
            else
            {
                return new OkObjectResult(renderResult);
            }
        }

        [HttpDelete("libraries/{libraryId}/categories/{categoryId}", Name = nameof(CategoryController.DeleteCategory))]
        [Authorize(Role.Admin, Role.LibraryAdmin)]
        public async Task<IActionResult> DeleteCategory(int libraryId, int categoryId, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteCategoryRequest(libraryId, categoryId);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }
    }
}
