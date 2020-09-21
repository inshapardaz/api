using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Handlers;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers
{
    public class LibraryController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderLibrary _libraryRenderer;
        private readonly IUserHelper _userHelper;

        public LibraryController(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor, IRenderLibrary libraryRenderer, IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _libraryRenderer = libraryRenderer;
            _userHelper = userHelper;
        }

        [HttpGet("library/{libraryId}", Name = nameof(LibraryController.GetLibraryById))]
        public async Task<IActionResult> GetLibraryById(int libraryId, CancellationToken cancellationToken)
        {
            var query = new GetLibraryQuery(libraryId, _userHelper.Claims);
            var library = await _queryProcessor.ExecuteAsync(query, cancellationToken);

            if (library != null)
            {
                return new OkObjectResult(_libraryRenderer.Render(library));
            }

            return NotFound();
        }

        [HttpPost("library", Name = nameof(LibraryController.CreateLibrary))]
        public async Task<IActionResult> CreateLibrary([FromBody]LibraryView library, CancellationToken token)
        {
            var request = new AddLibraryRequest(_userHelper.Claims, library.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _libraryRenderer.Render(request.Result);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        [HttpPut("library/{libraryId}", Name = nameof(LibraryController.UpdateLibrary))]
        public async Task<IActionResult> UpdateLibrary(int libraryId, [FromBody]LibraryView library, CancellationToken token = default(CancellationToken))
        {
            var request = new UpdateLibraryRequest(_userHelper.Claims, libraryId, library.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _libraryRenderer.Render(request.Result.Library);
            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }
            else
            {
                return new OkObjectResult(renderResult);
            }
        }

        [HttpDelete("library/{libraryId}", Name = nameof(LibraryController.DeleteLibrary))]
        public async Task<IActionResult> DeleteLibrary(int libraryId, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteLibraryRequest(_userHelper.Claims, libraryId);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }
    }
}
