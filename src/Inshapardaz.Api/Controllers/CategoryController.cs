using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IUserHelper _userHelper;

        public CategoryController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderCategory categoryRenderer,
            IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _categoryRenderer = categoryRenderer;
            _userHelper = userHelper;
        }

        [HttpGet("library/{libraryId}/categories", Name = nameof(CategoryController.GetCategories))]
        public async Task<IActionResult> GetCategories(int libraryId, CancellationToken token = default(CancellationToken))
        {
            var query = new GetCategoriesQuery(libraryId);
            var categories = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            return new OkObjectResult(_categoryRenderer.Render(categories, libraryId));
        }

        [HttpGet("library/{libraryId}/categories/{categoryId}", Name = nameof(CategoryController.GetCategoryById))]
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

        [HttpPost("library/{libraryId}/categories", Name = nameof(CategoryController.CreateCategory))]
        [Authorize]
        public async Task<IActionResult> CreateCategory(int libraryId, CategoryView category, CancellationToken token = default(CancellationToken))
        {
            var request = new AddCategoryRequest(_userHelper.Claims, libraryId, category.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _categoryRenderer.Render(request.Result, libraryId);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        [HttpPut("library/{libraryId}/categories/{categoryId}", Name = nameof(CategoryController.UpdateCategory))]
        [Authorize]
        public async Task<IActionResult> UpdateCategory(int libraryId, int categoryId, CategoryView category, CancellationToken token = default(CancellationToken))
        {
            category.Id = categoryId;
            var request = new UpdateCategoryRequest(_userHelper.Claims, libraryId, category.Map());
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

        [HttpDelete("library/{libraryId}/categories/{categoryId}", Name = nameof(CategoryController.DeleteCategory))]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int libraryId, int categoryId, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteCategoryRequest(_userHelper.Claims, libraryId, categoryId);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }
    }
}
