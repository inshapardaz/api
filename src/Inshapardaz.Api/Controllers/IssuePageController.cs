using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Page;
using Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue.Page;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

public class IssuePageController : Controller
{
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IRenderIssuePage _issuePageRenderer;
    private readonly IRenderFile _fileRenderer;
    private readonly IUserHelper _userHelper;

    public IssuePageController(IAmACommandProcessor commandProcessor,
        IQueryProcessor queryProcessor,
        IRenderIssuePage issuePageRenderer,
        IRenderFile fileRenderer,
        IUserHelper userHelper)
    {
        _commandProcessor = commandProcessor;
        _queryProcessor = queryProcessor;
        _issuePageRenderer = issuePageRenderer;
        _fileRenderer = fileRenderer;
        _userHelper = userHelper;
    }

    [HttpGet("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages", Name = nameof(IssuePageController.GetPagesByIssue))]
    [Produces(typeof(PageView<IssuePageView>))]
    public async Task<IActionResult> GetPagesByIssue(int libraryId,
        int periodicalId,
        int volumeNumber,
        int issueNumber,
        int pageNumber = 1,
        int pageSize = 10,
        [FromQuery] EditingStatus status = EditingStatus.All,
        [FromQuery] AssignmentFilter writerAssignmentFilter = AssignmentFilter.All,
        [FromQuery] AssignmentFilter reviewerAssignmentFilter = AssignmentFilter.All,
        [FromQuery] int? assignmentTo = null,
        CancellationToken token = default(CancellationToken))
    {
        var query = new GetIssuePagesQuery(libraryId, periodicalId, volumeNumber, issueNumber, pageNumber, pageSize)
        {
            StatusFilter = status,
            WriterAssignmentFilter = writerAssignmentFilter,
            ReviewerAssignmentFilter = reviewerAssignmentFilter,
            AccountId = writerAssignmentFilter == AssignmentFilter.AssignedToMe || reviewerAssignmentFilter == AssignmentFilter.AssignedToMe ? _userHelper.AccountId : assignmentTo
        };
        var result = await _queryProcessor.ExecuteAsync(query, token);

        var args = new PageRendererArgs<IssuePageModel, PageFilter>
        {
            Page = result,
            RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
            Filters = new PageFilter { Status = status, AssignmentFilter = writerAssignmentFilter, ReviewerAssignmentFilter = reviewerAssignmentFilter, AccountId = assignmentTo }
        };

        return new OkObjectResult(_issuePageRenderer.Render(args, libraryId, periodicalId, volumeNumber, issueNumber));
    }

    [HttpGet("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages/{sequenceNumber}", Name = nameof(IssuePageController.GetIssuePageByIndex))]
    [Produces(typeof(IssuePageView))]
    public async Task<IActionResult> GetIssuePageByIndex(int libraryId,
        int periodicalId,
        int volumeNumber,
        int issueNumber,
        int sequenceNumber,
        CancellationToken token = default(CancellationToken))
    {
        var request = new GetIssuePageByNumberQuery(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber);

        var result = await _queryProcessor.ExecuteAsync(request, cancellationToken: token);

        if (result == null)
        {
            return NotFound();
        }

        var renderResult = _issuePageRenderer.Render(result, libraryId);
        return Ok(renderResult);
    }

    [HttpPost("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages", Name = nameof(IssuePageController.CreateIssuePage))]
    [Produces(typeof(IssuePageView))]
    public async Task<IActionResult> CreateIssuePage(int libraryId,
        int periodicalId,
        int volumeNumber,
        int issueNumber,
        [FromBody] IssuePageView page,
        CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var model = page.Map();

        var request = new AddIssuePageRequest(libraryId, periodicalId, volumeNumber, issueNumber, _userHelper.AccountId, model);

        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = _issuePageRenderer.Render(request.Result, libraryId);
        
        if (request.IsAdded)
        {
            return Created(renderResult.Links.Self(), renderResult);
        }

        return Ok(renderResult);
    }

    [HttpPost("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages/{sequenceNumber}/sequenceNumber", Name = nameof(IssuePageController.UpdateIssuePageSequence))]
    public async Task<IActionResult> UpdateIssuePageSequence(int libraryId,
        int periodicalId,
        int volumeNumber,
        int issueNumber,
        int sequenceNumber,
        [FromBody] IssuePageView page,
        CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new UpdateIssuePageSequenceRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, page.SequenceNumber);
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        return Ok();
    }

    [HttpPost("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages/upload", Name = nameof(IssuePageController.UploadIssuePages))]
    [RequestSizeLimit(long.MaxValue)]
    public async Task<IActionResult> UploadIssuePages(int libraryId,
        int periodicalId,
        int volumeNumber,
        int issueNumber,
        CancellationToken token = default(CancellationToken))
    {
        List<IFormFile> files = Request.Form.Files.ToList();
        var fileModels = new List<FileModel>();
        foreach (var file in files)
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            fileModels.Add(new FileModel
            {
                FileName = file.FileName,
                MimeType = file.ContentType,
                Contents = content
            });
        }

        var request = new UploadIssuePages(libraryId, periodicalId, volumeNumber, issueNumber)
        {
            Files = fileModels
        };

        await _commandProcessor.SendAsync(request, cancellationToken: token);

        return new OkResult();
    }

    [HttpPut("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages/{sequenceNumber}", Name = nameof(IssuePageController.UpdateIssuePage))]
    [Produces(typeof(IssuePageView))]
    public async Task<IActionResult> UpdateIssuePage(int libraryId,
        int periodicalId,
        int volumeNumber,
        int issueNumber,
        int sequenceNumber,
        [FromBody] IssuePageView page,
        CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var model = page.Map();
        model.PeriodicalId = periodicalId;
        model.VolumeNumber = volumeNumber;
        model.IssueNumber = issueNumber;
        model.SequenceNumber = sequenceNumber;
        var request = new UpdateIssuePageRequest(libraryId, model);

        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = _issuePageRenderer.Render(request.Result.IssuePage, libraryId);

        if (request.Result.HasAddedNew)
        {
            return Created(renderResult.Links.Self(), renderResult);
        }

        return Ok(renderResult);
    }

    [HttpDelete("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages/{sequenceNumber}", Name = nameof(IssuePageController.DeleteIssuePage))]
    public async Task<IActionResult> DeleteIssuePage(int libraryId,
        int periodicalId,
        int volumeNumber,
        int issueNumber,
        int sequenceNumber,
        CancellationToken token = default(CancellationToken))
    {
        var request = new DeleteIssuePageRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber);
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        return Ok();
    }

    [HttpPost("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages/{sequenceNumber}/ocr", Name = nameof(IssuePageController.OcrIssuePage))]
    public async Task<IActionResult> OcrIssuePage(int libraryId,
        int periodicalId,
        int volumeNumber,
        int issueNumber,
        int sequenceNumber,
        [FromBody] string apiKey,
        CancellationToken token = default(CancellationToken))
    {
        var request = new IssuePageOcrRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, apiKey);
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        return Ok();
    }

    [HttpPut("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages/{sequenceNumber}/image", Name = nameof(IssuePageController.UpdateIssuePageImage))]
    [Produces(typeof(IssuePageView))]
    public async Task<IActionResult> UpdateIssuePageImage(int libraryId,
        int periodicalId,
        int volumeNumber,
        int issueNumber,
        int sequenceNumber,
        IFormFile file,
        CancellationToken token = default(CancellationToken))
    {
        var content = new byte[file.Length];
        using (var stream = new MemoryStream(content))
        {
            await file.CopyToAsync(stream);
        }

        var request = new UpdateIssuePageImageRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber)
        {
            Image = new FileModel
            {
                FileName = file.FileName,
                MimeType = file.ContentType,
                Contents = content
            }
        };

        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var imageLink = _issuePageRenderer.RenderImageLink(libraryId, request.Result.File);

        if (request.Result.HasAddedNew)
        {
            return new CreatedResult(imageLink.Href, null);
        }

        return new OkResult();
    }

    [HttpDelete("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages/{sequenceNumber}/image", Name = nameof(IssuePageController.DeleteIssuePageImage))]
    public async Task<IActionResult> DeleteIssuePageImage(int libraryId,
        int periodicalId,
        int volumeNumber,
        int issueNumber,
        int sequenceNumber,
        CancellationToken token = default(CancellationToken))
    {
        var request = new DeleteIssuePageImageRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber);
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        return Ok();
    }

    [HttpPost("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages/{sequenceNumber}/assign/me", Name = nameof(IssuePageController.AssignIssuePageToUser))]
    [Produces(typeof(IssuePageView))]
    public async Task<IActionResult> AssignIssuePageToUser(int libraryId,
        int periodicalId,
        int volumeNumber,
        int issueNumber,
        int sequenceNumber,
        CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new AssignIssuePageToUserRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, _userHelper.AccountId);

        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = _issuePageRenderer.Render(request.Result, libraryId);

        return Ok(renderResult);
    }

    [HttpPost("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages/{sequenceNumber}/assign", Name = nameof(IssuePageController.AssignIssuePage))]
    [Produces(typeof(IssuePageView))]
    public async Task<IActionResult> AssignIssuePage(int libraryId,
        int periodicalId,
        int volumeNumber,
        int issueNumber,
        int sequenceNumber,
        [FromBody] PageAssignmentView assignment,
        CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new AssignIssuePageRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, assignment.AccountId);

        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = _issuePageRenderer.Render(request.Result, libraryId);

        return Ok(renderResult);
    }
}
