using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
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

        [HttpGet("libraries", Name = nameof(LibraryController.GetLibraries))]
        public async Task<IActionResult> GetLibraries(string query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var libQuery = new GetLibrariesQuery(pageNumber, pageSize, _userHelper.Account?.Id, _userHelper.Account?.Role) { Query = query };
            var libraries = await _queryProcessor.ExecuteAsync(libQuery, cancellationToken: cancellationToken);

            var args = new PageRendererArgs<LibraryModel>
            {
                Page = libraries,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
            };

            return new OkObjectResult(_libraryRenderer.Render(args));
        }

        [HttpGet("libraries/{libraryId}", Name = nameof(LibraryController.GetLibraryById))]
        public async Task<IActionResult> GetLibraryById(int libraryId, CancellationToken cancellationToken)
        {
            var query = new GetLibraryQuery(libraryId);
            var library = await _queryProcessor.ExecuteAsync(query, cancellationToken);

            if (library != null)
            {
                return new OkObjectResult(_libraryRenderer.Render(library));
            }

            return NotFound();
        }

        [HttpPost("libraries", Name = nameof(LibraryController.CreateLibrary))]
        [Authorize(Role.Admin)]
        public async Task<IActionResult> CreateLibrary([FromBody]LibraryView library, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AddLibraryRequest(library.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _libraryRenderer.Render(request.Result);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        [HttpPut("libraries/{libraryId}", Name = nameof(LibraryController.UpdateLibrary))]
        [Authorize(Role.Admin, Role.LibraryAdmin)]
        public async Task<IActionResult> UpdateLibrary(int libraryId, [FromBody]LibraryView library, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new UpdateLibraryRequest(libraryId, library.Map());
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

        [HttpDelete("libraries/{libraryId}", Name = nameof(LibraryController.DeleteLibrary))]
        [Authorize(Role.Admin)]
        public async Task<IActionResult> DeleteLibrary(int libraryId, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteLibraryRequest(libraryId);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [Authorize(Role.Admin)]
        [HttpGet("/accounts/{accountId}/libraries", Name = nameof(LibraryController.GetLibrariesByAccount))]
        public async Task<IActionResult> GetLibrariesByAccount(int accountId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var libQuery = new GetLibrariesQuery(pageNumber, pageSize, accountId, _userHelper.Account?.Role);
            var libraries = await _queryProcessor.ExecuteAsync(libQuery, cancellationToken: cancellationToken);

            var args = new PageRendererArgs<LibraryModel>
            {
                Page = libraries,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, AccountId = accountId },
            };

            return new OkObjectResult(_libraryRenderer.Render(args));
        }

        [Authorize(Role.Admin)]
        [HttpPost("/accounts/{accountId}/libraries/{libraryId}", Name = nameof(LibraryController.AddLibraryToAccount))]
        public async Task<IActionResult> AddLibraryToAccount(int accountId, int libraryId, CancellationToken token = default)
        {
            var request = new AddLibraryToAccountRequest(libraryId, accountId);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [Authorize(Role.Admin)]
        [HttpDelete("/accounts/{accountId}/libraries/{libraryId}", Name = nameof(LibraryController.RemoveLibraryFromAccount))]
        public async Task<IActionResult> RemoveLibraryFromAccount(int accountId, int libraryId, CancellationToken token = default)
        {
            var request = new RemoveLibraryFromAccountRequest(libraryId, accountId);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }
    }
}
