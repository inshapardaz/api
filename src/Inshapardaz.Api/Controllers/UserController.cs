using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library.Book;
using Inshapardaz.Domain.Ports.Query.Library;
using Inshapardaz.Domain.Ports.Query.Library.Book;
using Inshapardaz.Domain.Ports.Query.Library.Book.Page;
using Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue.Page;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

public class UserController(
    IAmACommandProcessor commandProcessor,
    IQueryProcessor queryProcessor,
    IRenderBook bookRenderer,
    IRenderBookPage bookPageRenderer,
    IRenderIssuePage issuePageRenderer,
    IRenderFile fileRenderer,
    IUserHelper userHelper)
    : Controller
{
    private readonly IRenderFile _fileRenderer = fileRenderer;

    [HttpGet("libraries/{libraryId}/my/summary", Name = nameof(UserController.GetUserPublicationSummary))]
    [Produces(typeof(IEnumerable<UserPageSummaryView>))]
    public async Task<IActionResult> GetUserPublicationSummary(int libraryId, CancellationToken token = default(CancellationToken))
    {
        var getBookPagesQuery = new GetUserPublicationSummary(libraryId, userHelper.AccountId.Value);
        var result = await queryProcessor.ExecuteAsync(getBookPagesQuery, token);

        return new OkObjectResult(result.Select(x => x.Map()));
    }

    [HttpGet("libraries/{libraryId}/books/my/pages", Name = nameof(UserController.GetBookPagesByUser))]
    [Produces(typeof(PageView<BookPageView>))]
    public async Task<IActionResult> GetBookPagesByUser(int libraryId,
        int pageNumber = 1,
        int pageSize = 10,
        [FromQuery] EditingStatus status = EditingStatus.All,
        [FromQuery] AssignmentFilter assignmentFilter = AssignmentFilter.All,
        CancellationToken token = default(CancellationToken))
    {
        var getBookPagesQuery = new GetBookPagesForUserQuery(libraryId, userHelper.AccountId.Value, pageNumber, pageSize)
        {
            StatusFilter = status,
        };
        var result = await queryProcessor.ExecuteAsync(getBookPagesQuery, token);

        var args = new PageRendererArgs<BookPageModel, PageFilter>
        {
            Page = result,
            RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
            Filters = new PageFilter { Status = status }
        };

        return new OkObjectResult(bookPageRenderer.RenderUserPages(args, libraryId));
    }


    [HttpGet("libraries/{libraryId}/periodicals/my/pages", Name = nameof(UserController.GetIssuePagesByUser))]
    [Produces(typeof(PageView<BookPageView>))]
    public async Task<IActionResult> GetIssuePagesByUser(int libraryId,
       int pageNumber = 1,
       int pageSize = 10,
       [FromQuery] EditingStatus status = EditingStatus.All,
       [FromQuery] AssignmentFilter assignmentFilter = AssignmentFilter.All,
       CancellationToken token = default(CancellationToken))
    {
        var getBookPagesQuery = new GetIssuePagesForUserQuery(libraryId, userHelper.AccountId.Value, pageNumber, pageSize)
        {
            StatusFilter = status,
        };
        var result = await queryProcessor.ExecuteAsync(getBookPagesQuery, token);

        var args = new PageRendererArgs<IssuePageModel, PageFilter>
        {
            Page = result,
            RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
            Filters = new PageFilter { Status = status }
        };

        return new OkObjectResult(issuePageRenderer.RenderUserPages(args, libraryId));
    }

    [HttpGet("libraries/{libraryId}/my/books", Name = nameof(UserController.GetBooksByUser))]
    [Produces(typeof(PageView<BookView>))]
    public async Task<IActionResult> GetBooksByUser(int libraryId,
        int pageNumber = 1,
        int pageSize = 10,
        [FromQuery] StatusType status = StatusType.BeingTyped,
        CancellationToken token = default(CancellationToken))
    {
        var getBookPagesQuery = new GetUserBooksQuery(libraryId, userHelper.AccountId.Value, pageNumber, pageSize)
        {
            StatusFilter = status,
        };
        var result = await queryProcessor.ExecuteAsync(getBookPagesQuery, token);

        var args = new PageRendererArgs<BookModel, BookFilter>
        {
            Page = result,
            RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
            Filters = new BookFilter { Status = status }
        };

        return new OkObjectResult(bookRenderer.Render(args, libraryId));
    }
    
    [HttpPost("libraries/{libraryId}/my/books/{bookId}", Name = nameof(UpdateUserBookProgress))]
    [Produces(typeof(ReadProgressView))]
    public async Task<IActionResult> UpdateUserBookProgress(int libraryId, 
        int bookId,
        [FromBody] ReadProgressView readStatus,
        CancellationToken token = default)
    {
        
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var updateBookProgressCommand = new UpdateBookProgressRequest(libraryId, userHelper.AccountId ?? 0, bookId, readStatus.Map());
        await commandProcessor.SendAsync(updateBookProgressCommand, cancellationToken: token);
    
        if (updateBookProgressCommand.Result?.Progress == null)
        {
            return NotFound();
        }
    
        return new OkObjectResult(updateBookProgressCommand.Result.Progress.Map());
    }
}
