using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers
{
    public class SeriesController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderSeries _seriesRenderer;
        private readonly IRenderFile _fileRenderer;
        private readonly IUserHelper _userHelper;

        public SeriesController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderSeries SeriesRenderer,
            IRenderFile fileRenderer,
            IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _seriesRenderer = SeriesRenderer;
            _fileRenderer = fileRenderer;
            _userHelper = userHelper;
        }

        [HttpGet("library/{libraryId}/series", Name = nameof(SeriesController.GetSereies))]
        public async Task<IActionResult> GetSereies(int libraryId, string query, int pageNumber = 1, int pageSize = 10, CancellationToken token = default(CancellationToken))
        {
            var seriesQuery = new GetSeriesQuery(libraryId, pageNumber, pageSize) { Query = query };
            var series = await _queryProcessor.ExecuteAsync(seriesQuery, cancellationToken: token);

            var args = new PageRendererArgs<SeriesModel>
            {
                Page = series,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
            };

            return new OkObjectResult(_seriesRenderer.Render(args, libraryId));
        }

        [HttpGet("library/{libraryId}/series/{seriesId}", Name = nameof(SeriesController.GetSeriesById))]
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

        [HttpPost("library/{libraryId}/series", Name = nameof(SeriesController.CreateSeries))]
        [Authorize]
        public async Task<IActionResult> CreateSeries(int libraryId, SeriesView series, CancellationToken token = default(CancellationToken))
        {
            var request = new AddSeriesRequest(_userHelper.Claims, libraryId, series.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _seriesRenderer.Render(request.Result, libraryId);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        [HttpPut("library/{libraryId}/series/{seriesId}", Name = nameof(SeriesController.UpdateSeries))]
        [Authorize]
        public async Task<IActionResult> UpdateSeries(int libraryId, int seriesId, SeriesView series, CancellationToken token = default(CancellationToken))
        {
            series.Id = seriesId;
            var request = new UpdateSeriesRequest(_userHelper.Claims, libraryId, series.Map());
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

        [HttpDelete("library/{libraryId}/series/{seriesId}", Name = nameof(SeriesController.DeleteSeries))]
        [Authorize]
        public async Task<IActionResult> DeleteSeries(int libraryId, int seriesId, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteSeriesRequest(_userHelper.Claims, libraryId, seriesId);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPut("library/{libraryId}/series/{seriesId}/image", Name = nameof(SeriesController.UpdateSeriesImage))]
        [Authorize]
        public async Task<IActionResult> UpdateSeriesImage(int libraryId, int seriesId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var request = new UpdateSeriesImageRequest(_userHelper.Claims, libraryId, seriesId)
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
