using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Controllers
{
    public class ToolController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderCorrection _correctionRenderer;

        public ToolController(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor, IRenderCorrection correctionRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _correctionRenderer = correctionRenderer;
        }

        [HttpGet("/tools/{language}/spellchecker/{profile}", Name = nameof(ToolController.GetAllCorrections))]
        public async Task<IActionResult> GetAllCorrections(string language, string profile, CancellationToken cancellationToken)
        {
            var query = new GetAllCorrectionsQuery() { Language = language, Profile = profile };
            var result = await _queryProcessor.ExecuteAsync(query, cancellationToken: cancellationToken);
            return Ok(result);
        }


        [HttpGet("/tools/{language}/corrections/{profile}", Name = nameof(ToolController.GetCorrections))]
        public async Task<IActionResult> GetCorrections(string language, string profile, string query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default(CancellationToken))
        {
            var correctionQuery = new GetCorrectionsQuery() { Language = language, Query = query, Profile = profile, PageNumber = pageNumber, PageSize = pageSize };
            var result = await _queryProcessor.ExecuteAsync(correctionQuery, cancellationToken: cancellationToken);
            var pageRenderArgs = new PageRendererArgs<CorrectionModel>
            {
                Page = result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
            };
            return Ok(_correctionRenderer.Render(pageRenderArgs, language, profile));
        }

        [HttpGet("/tools/{language}/corrections/{profile}/{id}", Name = nameof(ToolController.GetCorrectionById))]
        [Authorize(Role.Admin)]
        public async Task<IActionResult> GetCorrectionById(string language, string profile, long id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = new GetCorrectionQuery() { Id = id };
            var result = await _queryProcessor.ExecuteAsync(query, cancellationToken: cancellationToken);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(_correctionRenderer.Render(result));
        }


        [HttpPost("/tools/{language}/corrections/{profile}", Name = nameof(ToolController.AddCorrection))]
        [Authorize(Role.Admin)]
        public async Task<IActionResult> AddCorrection(string language, string profile, [FromBody] CorrectionView correction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            correction.Language = language;
            correction.Profile = profile;

            var request = new AddCorrectionRequest(correction.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var renderResult = _correctionRenderer.Render(request.Result);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }
         
        [HttpPut("/tools/{language}/corrections/{profile}/{id}", Name = nameof(ToolController.UpdateCorrection))]
        [Authorize(Role.Admin)]
        public async Task<IActionResult> UpdateCorrection(string language, string profile, long id, [FromBody] CorrectionView correction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            correction.Language = language;
            correction.Profile = profile;
            correction.Id = id;
            var request = new UpdateCorrectionRequest(correction.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var renderResult = _correctionRenderer.Render(request.Result.Correction);
            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }
            else
            {
                return new OkObjectResult(renderResult);
            }
        }

        [HttpDelete("/tools/{language}/corrections/{profile}/{id}", Name = nameof(ToolController.DeleteCorrection))]
        [Authorize(Role.Admin)]
        public async Task<IActionResult> DeleteCorrection(string language, string profile, long id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new DeleteCorrectionRequest(language, profile, id);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);
            return new NoContentResult();
        }
    }
}
