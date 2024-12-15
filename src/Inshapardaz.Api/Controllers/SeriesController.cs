using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library.Series;
using Inshapardaz.Domain.Ports.Query.Library.Series;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

public class SeriesController : Controller
{
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IRenderSeries _seriesRenderer;
    private readonly IRenderFile _fileRenderer;

    public SeriesController(IAmACommandProcessor commandProcessor,
        IQueryProcessor queryProcessor,
        IRenderSeries SeriesRenderer,
        IRenderFile fileRenderer)
    {
        _commandProcessor = commandProcessor;
        _queryProcessor = queryProcessor;
        _seriesRenderer = SeriesRenderer;
        _fileRenderer = fileRenderer;
    }

    [HttpGet("libraries/{libraryId}/series", Name = nameof(SeriesController.GetSeries))]
    public async Task<IActionResult> GetSeries(int libraryId, string query, int pageNumber = 1, int pageSize = 10, SeriesSortByType sortby = SeriesSortByType.Name, SortDirection sortDirection = SortDirection.Ascending, CancellationToken token = default(CancellationToken))
    {
        var seriesQuery = new GetSeriesQuery(libraryId, pageNumber, pageSize)
        {
            Query = query,
            SortBy = sortby,
            SortDirection = sortDirection
        };
        var series = await _queryProcessor.ExecuteAsync(seriesQuery, cancellationToken: token);

        var args = new PageRendererArgs<SeriesModel>
        {
            Page = series,
            RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
        };

        return new OkObjectResult(_seriesRenderer.Render(args, libraryId));
    }

    [HttpGet("libraries/{libraryId}/series/{seriesId}", Name = nameof(SeriesController.GetSeriesById))]
    public async Task<IActionResult> GetSeriesById(int libraryId, int seriesId, CancellationToken token = default(CancellationToken))
    {
        var query = new GetSeriesByIdQuery(libraryId, seriesId);
        var Series = await _queryProcessor.ExecuteAsync(query, token);

        if (Series == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(_seriesRenderer.Render(Series, libraryId));
    }

    [HttpPost("libraries/{libraryId}/series", Name = nameof(SeriesController.CreateSeries))]
    public async Task<IActionResult> CreateSeries(int libraryId, [FromBody] SeriesView series, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new AddSeriesRequest(libraryId, series.Map());
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = _seriesRenderer.Render(request.Result, libraryId);
        return new CreatedResult(renderResult.Links.Self(), renderResult);
    }

    [HttpPut("libraries/{libraryId}/series/{seriesId}", Name = nameof(SeriesController.UpdateSeries))]
    public async Task<IActionResult> UpdateSeries(int libraryId, int seriesId, [FromBody] SeriesView series, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        series.Id = seriesId;
        var request = new UpdateSeriesRequest(libraryId, series.Map());
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = _seriesRenderer.Render(request.Result.Series, libraryId);

        if (request.Result.HasAddedNew)
        {
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }
        else
        {
            return new OkObjectResult(renderResult);
        }
    }

    [HttpDelete("libraries/{libraryId}/series/{seriesId}", Name = nameof(SeriesController.DeleteSeries))]
    public async Task<IActionResult> DeleteSeries(int libraryId, int seriesId, CancellationToken token = default(CancellationToken))
    {
        var request = new DeleteSeriesRequest(libraryId, seriesId);
        await _commandProcessor.SendAsync(request, cancellationToken: token);
        return new NoContentResult();
    }

    [HttpPut("libraries/{libraryId}/series/{seriesId}/image", Name = nameof(SeriesController.UpdateSeriesImage))]
    public async Task<IActionResult> UpdateSeriesImage(int libraryId, int seriesId, IFormFile file, CancellationToken token = default(CancellationToken))
    {
        var content = new byte[file.Length];
        using (var stream = new MemoryStream(content))
        {
            await file.CopyToAsync(stream);
        }

        var request = new UpdateSeriesImageRequest(libraryId, seriesId)
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
            var response = _fileRenderer.Render(libraryId, request.Result.File);
            return new CreatedResult(response.Links.Self(), response);
        }

        return new OkResult();
    }
}
