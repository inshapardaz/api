using System.IO;
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
using Inshapardaz.Domain.Ports.Handlers.Library;
using Microsoft.AspNetCore.Http;
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
        private readonly IRenderFile _fileRenderer;

        public LibraryController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderLibrary libraryRenderer,
            IUserHelper userHelper,
            IRenderFile fileRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _libraryRenderer = libraryRenderer;
            _userHelper = userHelper;
            _fileRenderer = fileRenderer;
        }

        [HttpGet("libraries", Name = nameof(LibraryController.GetLibraries))]
        [Produces(typeof(PageView<LibraryView>))]
        public async Task<IActionResult> GetLibraries(string query, int pageNumber = 1, bool assigned = false, bool unassigned = false, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            Role? role = _userHelper.Account != null && _userHelper.Account.IsSuperAdmin ? Role.Admin : null;
            var libQuery = new GetLibrariesQuery(pageNumber, pageSize, _userHelper.Account?.Id, role) { 
                Query = query, Assigned = assigned, 
                Unassigned = unassigned 
            };
            var libraries = await _queryProcessor.ExecuteAsync(libQuery, cancellationToken: cancellationToken);

            var args = new PageRendererArgs<LibraryModel>
            {
                Page = libraries,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
            };

            return new OkObjectResult(_libraryRenderer.Render(args));
        }

        [HttpGet("libraries/{libraryId}", Name = nameof(LibraryController.GetLibraryById))]
        [Produces(typeof(LibraryView))]
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
        public async Task<IActionResult> CreateLibrary([FromBody] LibraryView library, CancellationToken token)
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
        public async Task<IActionResult> UpdateLibrary(int libraryId, [FromBody] LibraryView library, CancellationToken token = default(CancellationToken))
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
            Role? role = _userHelper.Account != null && _userHelper.Account.IsSuperAdmin ? Role.Admin : null;
            var libQuery = new GetLibrariesQuery(pageNumber, pageSize, accountId, role);
            var libraries = await _queryProcessor.ExecuteAsync(libQuery, cancellationToken: cancellationToken);

            var args = new PageRendererArgs<LibraryModel>
            {
                Page = libraries,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, AccountId = accountId },
            };

            return new OkObjectResult(_libraryRenderer.Render(args));
        }

        [Authorize(Role.Admin)]
        [HttpPost("/accounts/{accountId}/libraries", Name = nameof(LibraryController.AddLibraryToAccount))]

        // TODO : Add role to the body
        public async Task<IActionResult> AddLibraryToAccount(int accountId, [FromBody] int libraryId, CancellationToken token = default)
        {
            var request = new AddLibraryToAccountRequest(libraryId, accountId, Role.Reader);
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

        [HttpPut("libraries/{libraryId}/image", Name = nameof(LibraryController.UpdateLibraryImage))]
        [Authorize(Role.Admin, Role.LibraryAdmin)]
        public async Task<IActionResult> UpdateLibraryImage(int libraryId, [FromForm] IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var request = new UpdateLibraryImageRequest(libraryId)
            {
                Image = new FileModel
                {
                    FileName = file.FileName,
                    MimeType = file.ContentType,
                    Contents = content
                }
            };

            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result.HasAddedNew)
            {
                var response = _fileRenderer.Render(libraryId, request.Result.File);
                return new CreatedResult(response.Links.Self(), response);
            }

            return new OkResult();
        }
    }
}
