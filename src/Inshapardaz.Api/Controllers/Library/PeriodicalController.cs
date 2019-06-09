using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Library;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Ports.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Controllers.Library
{
    public class PeriodicalController : Controller
    {
        private IAmACommandProcessor _commandProcessor;
        private readonly IRenderPeriodicals _periodicalsRenderer;
        private readonly IRenderPeriodical _periodicalRenderer;
        private readonly IRenderFile _fileRenderer;

        public PeriodicalController(IAmACommandProcessor commandProcessor, 
            IRenderPeriodicals periodicalsRenderer, 
            IRenderPeriodical periodicalRenderer,
            IRenderFile fileRenderer)
        {
            _commandProcessor = commandProcessor;
            _periodicalsRenderer = periodicalsRenderer;
            _periodicalRenderer = periodicalRenderer;
            _fileRenderer = fileRenderer;
        }

        [HttpGet("/api/periodicals", Name = "GetPeriodicals")]
        [Produces(typeof(IEnumerable<PeriodicalView>))]
        public async Task<IActionResult> GetList(string query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new GetPeriodicalsRequest(pageNumber, pageSize)
            {
                Query = query
            };
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var args = new PageRendererArgs<Periodical>
            {
                Page = request.Result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                RouteName = "GetPeriodicals"
            };

            return Ok(_periodicalsRenderer.Render(args));
        }

        [HttpGet("/api/periodicals/{id}", Name = "GetPeriodicalById")]
        [Produces(typeof(PeriodicalView))]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new GetPeriodicalByIdRequest(id);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result != null)
                return Ok(_periodicalRenderer.Render(request.Result));
            return NotFound();
        }

        [Authorize]
        [HttpPost("/api/periodicals", Name = "CreatePeriodical")]
        [Produces(typeof(PeriodicalView))]
        public async Task<IActionResult> Post([FromBody]PeriodicalView value, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new AddPeriodicalRequest(value.Map<PeriodicalView, Periodical>());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var renderResult = _periodicalRenderer.Render(request.Result);
            return Created(renderResult.Links.Self(), renderResult);
        }

        [Authorize]
        [HttpPut("/api/periodicals/{id}", Name = "UpdatePeriodical")]
        [Produces(typeof(PeriodicalView))]
        public async Task<IActionResult> Put(int id, [FromBody]PeriodicalView value, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new UpdatePeriodicalRequest(value.Map<PeriodicalView, Periodical>());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result.HasAddedNew)
            {
                var renderResult = _periodicalRenderer.Render(request.Result.Periodical);
                return Created(renderResult.Links.Self(), renderResult);
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("/api/periodicals/{id}", Name = "DeletePeriodical")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new DeletePeriodicalRequest(id);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);
            return NoContent();
        }

        [Authorize]
        [HttpPost("/api/periodicals/{id}/image", Name = "UpdatePeriodicalImage")]
        [ValidateModel]
        public async Task<IActionResult> PutImage(int id, IFormFile file, CancellationToken cancellationToken)
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var request = new UpdatePeriodicalImageRequest(id)
            {
                Image = new Domain.Entities.File
                {
                    FileName = file.FileName,
                    MimeType = file.ContentType,
                    Contents = content
                }
            };

            await _commandProcessor.SendAsync(request);


            if (request.Result.HasAddedNew)
            {
                var response = _fileRenderer.Render(request.Result.File);
                return Created(response.Links.Self(), response);
            }

            return NoContent();
        }
    }
}
