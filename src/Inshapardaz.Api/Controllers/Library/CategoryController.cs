using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers.Library;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Ports.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers.Library
{
    public class CategoryController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderCategories _renderCategories;
        private readonly IRenderCategory _renderCategory;

        public CategoryController(IAmACommandProcessor commandProcessor, IRenderCategories renderCategories, IRenderCategory renderCategory)
        {
            _commandProcessor = commandProcessor;
            _renderCategories = renderCategories;
            _renderCategory = renderCategory;
        }

        [HttpGet("/api/categories", Name = "GetCategories")]
        [Produces(typeof(IEnumerable<CategoryView>))]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var request = new GetCategoryRequest();
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            return Ok(_renderCategories.RenderResult(request.Result));
        }

        [HttpGet("/api/categories/{id}", Name = "GetCategoryById")]
        [Produces(typeof(CategoryView))]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var request = new GetCategoryByIdRequest(id);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result != null)
                return Ok(_renderCategory.RenderResult(request.Result));
            return NotFound();
        }

        [Authorize]
        [HttpPost("/api/categories", Name = "CreateCategory")]
        [Produces(typeof(CategoryView))]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody]CategoryView value, CancellationToken cancellationToken)
        {
            var request = new AddCategoryRequest(value.Map<CategoryView, Category>());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var renderResult = _renderCategory.RenderResult(request.Result);
            return Created(renderResult.Links.Self(), renderResult);
        }

        [Authorize]
        [HttpPut("/api/categories/{id}", Name = "UpdateCategory")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, [FromBody] CategoryView value, CancellationToken cancellationToken)
        {
            var request = new UpdateCategoryRequest(value.Map<CategoryView, Category>());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result.HasAddedNew)
            {
                var renderResult = _renderCategory.RenderResult(request.Result.Category);
                return Created(renderResult.Links.Self(), renderResult);
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("/api/categories/{id}", Name = "DeleteCategory")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var request = new DeleteCategoryRequest(id);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);
            return NoContent();
        }

    }
}
