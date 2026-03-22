using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library.Book.Page;
using Inshapardaz.Domain.Ports.Query.Library.Book.Page;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

public class BookPageController(
    IAmACommandProcessor commandProcessor,
    IQueryProcessor queryProcessor,
    IRenderBookPage bookPageRenderer,
    IRenderFile fileRenderer,
    IUserHelper userHelper)
    : Controller
{
    private readonly IRenderFile _fileRenderer = fileRenderer;

    [HttpGet("libraries/{libraryId}/books/{bookId}/pages", Name = nameof(BookPageController.GetPagesByBook))]
    [Produces(typeof(PageView<BookPageView>))]
    public async Task<IActionResult> GetPagesByBook(int libraryId,
        int bookId,
        int pageNumber = 1,
        int pageSize = 10,
        [FromQuery] EditingStatus status = EditingStatus.All,
        [FromQuery] AssignmentFilter assignmentFilter = AssignmentFilter.All,
        [FromQuery] AssignmentFilter reviewerAssignmentFilter = AssignmentFilter.All,
        [FromQuery] SortDirection sortDirection = SortDirection.Ascending,
        [FromQuery] int? assignmentTo = null,
        CancellationToken token = default(CancellationToken))
    {
        var getBookPagesQuery = new GetBookPagesQuery(libraryId, bookId, pageNumber, pageSize)
        {
            StatusFilter = status,
            AssignmentFilter = assignmentFilter,
            ReviewerAssignmentFilter = reviewerAssignmentFilter,
            SortDirection = sortDirection,
            AccountId = assignmentFilter == AssignmentFilter.AssignedToMe || reviewerAssignmentFilter == AssignmentFilter.AssignedToMe ? userHelper.AccountId : assignmentTo
        };
        var result = await queryProcessor.ExecuteAsync(getBookPagesQuery, token);

        var args = new PageRendererArgs<BookPageModel, PageFilter>
        {
            Page = result,
            RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
            Filters = new PageFilter { Status = status, AssignmentFilter = assignmentFilter, ReviewerAssignmentFilter = reviewerAssignmentFilter, AccountId = assignmentTo }
        };

        return new OkObjectResult(bookPageRenderer.Render(args, libraryId, bookId));
    }

    [HttpGet("libraries/{libraryId}/books/{bookId}/pages/{sequenceNumber}", Name = nameof(BookPageController.GetPageByIndex))]
    [Produces(typeof(BookPageView))]
    public async Task<IActionResult> GetPageByIndex(int libraryId, int bookId, int sequenceNumber, CancellationToken token = default(CancellationToken))
    {
        var request = new GetBookPageByNumberQuery(libraryId, bookId, sequenceNumber);

        var result = await queryProcessor.ExecuteAsync(request, cancellationToken: token);

        if (result == null)
        {
            return NotFound();
        }

        var renderResult = bookPageRenderer.Render(result, libraryId);
        return Ok(renderResult);
    }

    [HttpPost("libraries/{libraryId}/books/{bookId}/pages", Name = nameof(BookPageController.CreatePage))]
    [Produces(typeof(BookPageView))]
    public async Task<IActionResult> CreatePage(int libraryId, int bookId, [FromBody] BookPageView page, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var model = page.Map();

        var request = new AddBookPageRequest(libraryId, bookId, userHelper.AccountId, model);

        await commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = bookPageRenderer.Render(request.Result, libraryId);

        //TODO: Remove updation
        if (request.IsAdded)
        {
            return Created(renderResult.Links.Self(), renderResult);
        }

        return Ok(renderResult);
    }

    [HttpPost("libraries/{libraryId}/books/{bookId}/pages/{sequenceNumber}/sequenceNumber", Name = nameof(BookPageController.UpdatePageSequence))]
    public async Task<IActionResult> UpdatePageSequence(int libraryId, int bookId, int sequenceNumber, [FromBody] BookPageView page, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new UpdateBookPageSequenceRequest(libraryId, bookId, sequenceNumber, page.SequenceNumber);
        await commandProcessor.SendAsync(request, cancellationToken: token);

        return Ok();
    }

    [HttpPost("libraries/{libraryId}/books/{bookId}/pages/upload", Name = nameof(BookPageController.UploadPages))]
    [RequestSizeLimit(long.MaxValue)]
    public async Task<IActionResult> UploadPages(int libraryId, int bookId, CancellationToken token = default(CancellationToken))
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

        var request = new UploadBookPagesRequest(libraryId, bookId)
        {
            Files = fileModels
        };

        await commandProcessor.SendAsync(request, cancellationToken: token);

        return new OkResult();
    }

    [HttpPut("libraries/{libraryId}/books/{bookId}/pages/{sequenceNumber}", Name = nameof(BookPageController.UpdatePage))]
    [Produces(typeof(BookPageView))]
    public async Task<IActionResult> UpdatePage(int libraryId, int bookId, int sequenceNumber, [FromBody] BookPageView page, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var model = page.Map();

        var request = new UpdateBookPageRequest(libraryId, bookId, sequenceNumber, userHelper.AccountId, model);

        await commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = bookPageRenderer.Render(request.Result.BookPage, libraryId);

        if (request.Result.HasAddedNew)
        {
            return Created(renderResult.Links.Self(), renderResult);
        }

        return Ok(renderResult);
    }

    [HttpDelete("libraries/{libraryId}/books/{bookId}/pages/{sequenceNumber}", Name = nameof(BookPageController.DeletePage))]
    public async Task<IActionResult> DeletePage(int libraryId, int bookId, int sequenceNumber, CancellationToken token = default(CancellationToken))
    {
        var request = new DeleteBookPageRequest(libraryId, bookId, sequenceNumber);
        await commandProcessor.SendAsync(request, cancellationToken: token);

        return Ok();
    }

    [HttpPost("libraries/{libraryId}/books/{bookId}/pages/{sequenceNumber}/ocr", Name = nameof(BookPageController.OcrPage))]
    public async Task<IActionResult> OcrPage(int libraryId, int bookId, int sequenceNumber, [FromBody] OcrRequest ocrRequest, CancellationToken token = default(CancellationToken))
    {
        var request = new BookPageOcrRequest(libraryId, bookId, sequenceNumber, ocrRequest.Key);
        await commandProcessor.SendAsync(request, cancellationToken: token);

        return Ok();
    }

    [HttpPut("libraries/{libraryId}/books/{bookId}/pages/{sequenceNumber}/image", Name = nameof(BookPageController.UpdatePageImage))]
    [Produces(typeof(BookPageView))]
    public async Task<IActionResult> UpdatePageImage(int libraryId, int bookId, int sequenceNumber, IFormFile file, CancellationToken token = default(CancellationToken))
    {
        var content = new byte[file.Length];
        using (var stream = new MemoryStream(content))
        {
            await file.CopyToAsync(stream);
        }

        var request = new UpdateBookPageImageRequest(libraryId, bookId, sequenceNumber)
        {
            Image = new FileModel
            {
                FileName = file.FileName,
                MimeType = file.ContentType,
                Contents = content
            }
        };

        await commandProcessor.SendAsync(request, cancellationToken: token);

        var imageLink = bookPageRenderer.RenderImageLink(libraryId, request.Result.File);

        if (request.Result.HasAddedNew)
        {
            return new CreatedResult(imageLink.Href, null);
        }

        return new OkResult();
    }

    [HttpDelete("libraries/{libraryId}/books/{bookId}/pages/{sequenceNumber}/image", Name = nameof(BookPageController.DeletePageImage))]
    public async Task<IActionResult> DeletePageImage(int libraryId, int bookId, int sequenceNumber, CancellationToken token = default(CancellationToken))
    {
        var request = new DeleteBookPageImageRequest(libraryId, bookId, sequenceNumber);
        await commandProcessor.SendAsync(request, cancellationToken: token);

        return Ok();
    }

    [HttpPost("libraries/{libraryId}/books/{bookId}/pages/{sequenceNumber}/assign/me", Name = nameof(BookPageController.AssignPageToUser))]
    [Produces(typeof(BookPageView))]
    public async Task<IActionResult> AssignPageToUser(int libraryId, int bookId, int sequenceNumber, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new AssignBookPageToUserRequest(libraryId, bookId, sequenceNumber, userHelper.Account.Id);

        await commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = bookPageRenderer.Render(request.Result, libraryId);

        return Ok(renderResult);
    }

    [HttpPost("libraries/{libraryId}/books/{bookId}/pages/{sequenceNumber}/assign", Name = nameof(BookPageController.AssignPage))]
    [Produces(typeof(BookPageView))]
    public async Task<IActionResult> AssignPage(int libraryId, int bookId, int sequenceNumber, [FromBody] PageAssignmentView assignment, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new AssignBookPageRequest(libraryId, bookId, sequenceNumber, assignment.AccountId);

        await commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = bookPageRenderer.Render(request.Result, libraryId);

        return Ok(renderResult);
    }
}
