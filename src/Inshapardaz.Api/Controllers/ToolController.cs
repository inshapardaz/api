using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.Tools;
using Inshapardaz.Domain.Ports.Query.Tools;
using Inshapardaz.Domain.Ports.Query.Tools.CommonWords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

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
    
    [HttpGet("/tools/{language}/spellchecker/{profile}", Name = nameof(GetAllCorrections))]
    public async Task<IActionResult> GetAllCorrections(string language, string profile, CancellationToken cancellationToken)
    {
        var query = new GetAllCorrectionsQuery() { Language = language, Profile = profile };
        var result = await _queryProcessor.ExecuteAsync(query, cancellationToken: cancellationToken);
        return Ok(_correctionRenderer.RenderSimple(result));
    }


    [HttpGet("/tools/{language}/corrections/{profile}", Name = nameof(GetCorrections))]
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

    [HttpGet("/tools/{language}/corrections/{profile}/{id}", Name = nameof(GetCorrectionById))]
    public async Task<IActionResult> GetCorrectionById(string language, string profile, long id, CancellationToken cancellationToken = default(CancellationToken))
    {
        var query = new GetCorrectionQuery() { Id = id, Language = language, Profile = profile };
        var result = await _queryProcessor.ExecuteAsync(query, cancellationToken: cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(_correctionRenderer.Render(result));
    }


    [HttpPost("/tools/{language}/corrections/{profile}", Name = nameof(AddCorrection))]
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

    [HttpPut("/tools/{language}/corrections/{profile}/{id}", Name = nameof(UpdateCorrection))]
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

    [HttpDelete("/tools/{language}/corrections/{profile}/{id}", Name = nameof(DeleteCorrection))]
    public async Task<IActionResult> DeleteCorrection(string language, string profile, long id, CancellationToken cancellationToken = default(CancellationToken))
    {
        var request = new DeleteCorrectionRequest(language, profile, id);
        await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);
        return new NoContentResult();
    }

    [HttpPost("/tools/rekhtadownload", Name = nameof(DownloadRekhtaBook))]
    [RequestTimeout(500 * 1000)]
    public async Task<IActionResult> DownloadRekhtaBook([FromBody] RekhtaDownloadView downloadView, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var request = new DownloadRekhtaBookRequest(downloadView.BookUrl) { CreatePdf = downloadView.ConvertToPdf };
        await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

        return new FileContentResult(request.DownloadResult.File, request.DownloadResult.MimeType)
        {
            FileDownloadName = request.DownloadResult.FileName
        };
    }
    
    [HttpPost("/tools/chughtaidownload", Name = nameof(DownloadChughtaiBook))]
    [RequestTimeout(500 * 1000)]
    public async Task<IActionResult> DownloadChughtaiBook([FromBody] ChughtaiDownloadView downloadView, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var request = new DownloadChughtaiBookRequest(downloadView.BookUrl, downloadView.SessionId) { CreatePdf = downloadView.ConvertToPdf };
        await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

        return new FileContentResult(request.DownloadResult.File, request.DownloadResult.MimeType)
        {
            FileDownloadName = request.DownloadResult.FileName
        };
    }
}
