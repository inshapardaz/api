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
    public class SeriesController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderSeriesList _seriesListRenderer;
        private readonly IRenderSeries _seriesRenderer;

        public SeriesController(IAmACommandProcessor commandProcessor, IRenderSeries seriesRenderer, IRenderSeriesList seriesListRenderer)
        {
            _commandProcessor = commandProcessor;
            _seriesRenderer = seriesRenderer;
            _seriesListRenderer = seriesListRenderer;
        }

        [HttpGet("/api/series", Name = "GetSeries")]
        [Produces(typeof(IEnumerable<SeriesView>))]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var request = new GetSeriesRequest();
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            return Ok(_seriesListRenderer.RenderResult(request.Result));
        }

        [HttpGet("/api/series/{id}", Name = "GetSeriesById")]
        [Produces(typeof(SeriesView))]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var request = new GetSeriesByIdRequest(id);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result != null)
                return Ok(_seriesRenderer.RenderResult(request.Result));
            return NotFound();
        }

        [Authorize]
        [HttpPost("/api/series", Name = "CreateSeries")]
        [Produces(typeof(SeriesView))]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody]SeriesView value, CancellationToken cancellationToken)
        {
            var request = new AddSeriesRequest(value.Map<SeriesView, Series>());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var renderResult = _seriesRenderer.RenderResult(request.Result);
            return Created(renderResult.Links.Self(), renderResult);
        }

        [Authorize]
        [HttpPut("/api/series/{id}", Name = "UpdateSeries")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, [FromBody] SeriesView value, CancellationToken cancellationToken)
        {
            var request = new UpdateSeriesRequest(value.Map<SeriesView, Series>());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result.HasAddedNew)
            {
                var renderResult = _seriesRenderer.RenderResult(request.Result.Series);
                return Created(renderResult.Links.Self(), renderResult);
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("/api/series/{id}", Name = "DeleteSeries")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var request = new DeleteSeriesRequest(id);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);
            return NoContent();
        }

    }
}
