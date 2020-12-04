using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Controllers
{
    public class PeriodicalController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderPeriodical _periodicalRenderer;
        private readonly IRenderFile _fileRenderer;

        public PeriodicalController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderPeriodical periodicalRenderer,
            IRenderFile fileRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _periodicalRenderer = periodicalRenderer;
            _fileRenderer = fileRenderer;
        }

        [HttpGet("library/{libraryId}/periodicals", Name = nameof(PeriodicalController.GetPeriodicals))]
        public async Task<IActionResult> GetPeriodicals(int libraryId, string query, int pageNumber = 1, int pageSize = 10, CancellationToken token = default(CancellationToken))
        {
            var periodicalsQuery = new GetPeriodicalsQuery(libraryId, pageNumber, pageSize) { Query = query };
            var result = await _queryProcessor.ExecuteAsync(periodicalsQuery, token);

            var args = new PageRendererArgs<PeriodicalModel>
            {
                Page = result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
            };

            return new OkObjectResult(_periodicalRenderer.Render(args, libraryId));
        }

        [HttpGet("library/{libraryId}/periodicals/{periodicalId}", Name = nameof(PeriodicalController.GetPeriodicalById))]
        public async Task<IActionResult> GetPeriodicalById(int libraryId, int periodicalId, CancellationToken token = default(CancellationToken))
        {
            var query = new GetPeriodicalByIdQuery(libraryId, periodicalId);
            var periodical = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (periodical != null)
            {
                return new OkObjectResult(_periodicalRenderer.Render(periodical, libraryId));
            }

            return new NotFoundResult();
        }

        [HttpPost("library/{libraryId}/periodicals", Name = nameof(PeriodicalController.CreatePeriodical))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> CreatePeriodical(int libraryId, [FromBody]PeriodicalView periodical, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AddPeriodicalRequest(libraryId, periodical.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _periodicalRenderer.Render(request.Result, libraryId);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        [HttpPut("library/{libraryId}/periodicals/{periodicalId}", Name = nameof(PeriodicalController.UpdatePeriodical))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdatePeriodical(int libraryId, int periodicalId, [FromBody]PeriodicalView periodical, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new UpdatePeriodicalRequest(libraryId, periodical.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _periodicalRenderer.Render(request.Result.Periodical, libraryId);
            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }
            else
            {
                return new OkObjectResult(renderResult);
            }
        }

        [HttpDelete("library/{libraryId}/periodicals/{periodicalId}", Name = nameof(PeriodicalController.DeletePeriodical))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeletePeriodical(int libraryId, int periodicalId, CancellationToken token = default(CancellationToken))
        {
            var request = new DeletePeriodicalRequest(libraryId, periodicalId);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPut("library/{libraryId}/periodicals/{periodicalId}/image", Name = nameof(PeriodicalController.UpdatePeriodicalImage))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdatePeriodicalImage(int libraryId, int periodicalId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var request = new UpdatePeriodicalImageRequest(libraryId, periodicalId)
            {
                Image = new Domain.Models.FileModel
                {
                    FileName = file.FileName,
                    MimeType = file.ContentType,
                    Contents = content
                }
            };

            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result.HasAddedNew)
            {
                var response = _fileRenderer.Render(request.Result.File);
                return new CreatedResult(response.Links.Self(), response);
            }

            return new OkResult();
        }
    }
}
