using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Ports.Command.Library.Category;
using Inshapardaz.Domain.Ports.Query.Library.Category;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

public class CategoryController(
    IAmACommandProcessor commandProcessor,
    IQueryProcessor queryProcessor,
    IRenderCategory categoryRenderer)
    : Controller
{
    [HttpGet("libraries/{libraryId}/categories", Name = nameof(CategoryController.GetCategories))]
    public async Task<IActionResult> GetCategories(int libraryId, CancellationToken token = default(CancellationToken))
    {
        var query = new GetCategoriesQuery(libraryId);
        var categories = await queryProcessor.ExecuteAsync(query, cancellationToken: token);

        return new OkObjectResult(categoryRenderer.Render(categories, libraryId));
    }

    [HttpGet("libraries/{libraryId}/categories/tree", Name = nameof(CategoryController.GetCategoryTree))]
    public async Task<IActionResult> GetCategoryTree(int libraryId, CancellationToken token = default(CancellationToken))
    {
        var query = new GetCategoryTreeQuery(libraryId);
        var categories = await queryProcessor.ExecuteAsync(query, cancellationToken: token);

        return new OkObjectResult(categoryRenderer.Render(categories, libraryId));
    }

    [HttpGet("libraries/{libraryId}/categories/{categoryId}/children", Name = nameof(CategoryController.GetChildCategories))]
    public async Task<IActionResult> GetChildCategories(int libraryId, int categoryId, CancellationToken token = default(CancellationToken))
    {
        var query = new GetChildCategoriesQuery(libraryId, categoryId);
        var categories = await queryProcessor.ExecuteAsync(query, cancellationToken: token);

        return new OkObjectResult(categoryRenderer.Render(categories, libraryId));
    }

    [HttpGet("libraries/{libraryId}/categories/{categoryId}", Name = nameof(CategoryController.GetCategoryById))]
    public async Task<IActionResult> GetCategoryById(int libraryId, int categoryId, CancellationToken token = default(CancellationToken))
    {
        var query = new GetCategoryByIdQuery(libraryId, categoryId);
        var category = await queryProcessor.ExecuteAsync(query, token);

        if (category == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(categoryRenderer.Render(category, libraryId));
    }

    [HttpPost("libraries/{libraryId}/categories", Name = nameof(CategoryController.CreateCategory))]
    public async Task<IActionResult> CreateCategory(int libraryId, [FromBody] CategoryView category, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new AddCategoryRequest(libraryId, category.Map());
        await commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = categoryRenderer.Render(request.Result, libraryId);
        return new CreatedResult(renderResult.Links.Self(), renderResult);
    }

    [HttpPut("libraries/{libraryId}/categories/{categoryId}", Name = nameof(CategoryController.UpdateCategory))]
    public async Task<IActionResult> UpdateCategory(int libraryId, int categoryId, [FromBody] CategoryView category, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        category.Id = categoryId;
        var request = new UpdateCategoryRequest(libraryId, category.Map());
        await commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = categoryRenderer.Render(request.Result.Category, libraryId);

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
    public async Task<IActionResult> DeleteCategory(int libraryId, int categoryId, CancellationToken token = default(CancellationToken))
    {
        var request = new DeleteCategoryRequest(libraryId, categoryId);
        await commandProcessor.SendAsync(request, cancellationToken: token);
        return new NoContentResult();
    }
}
