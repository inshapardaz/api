using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library.Book;
using Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;
using Inshapardaz.Domain.Ports.Query.Library;
using Inshapardaz.Domain.Ports.Query.Library.Book;
using Inshapardaz.Domain.Ports.Query.Library.Book.Page;
using Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue;
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

    [HttpGet("libraries/{libraryId}/my/books/{bookId}/bookmarks", Name = nameof(GetUserBookmarks))]
    [Produces(typeof(IEnumerable<BookmarkView>))]
    public async Task<IActionResult> GetUserBookmarks(int libraryId, int bookId, CancellationToken token = default)
    {
        var query = new GetBookmarksQuery(libraryId, userHelper.AccountId ?? 0, bookId);
        var result = await queryProcessor.ExecuteAsync(query, token);

        return new OkObjectResult(result.Select(b => b.Map()));
    }

    [HttpPut("libraries/{libraryId}/my/books/{bookId}/bookmarks/{clientId}", Name = nameof(UpsertUserBookmark))]
    [Produces(typeof(BookmarkView))]
    public async Task<IActionResult> UpsertUserBookmark(int libraryId,
        int bookId,
        string clientId,
        [FromBody] BookmarkView bookmark,
        CancellationToken token = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var upsertBookmarkCommand = new UpsertBookmarkRequest(libraryId, userHelper.AccountId ?? 0, bookId, clientId, new BookmarkModel
        {
            ChapterId = bookmark.ChapterId,
            Position = bookmark.Position,
            Name = bookmark.Name,
        });
        await commandProcessor.SendAsync(upsertBookmarkCommand, cancellationToken: token);

        if (upsertBookmarkCommand.Result?.Bookmark == null)
        {
            return NotFound();
        }

        return new OkObjectResult(upsertBookmarkCommand.Result.Bookmark.Map());
    }

    [HttpDelete("libraries/{libraryId}/my/books/{bookId}/bookmarks/{clientId}", Name = nameof(DeleteUserBookmark))]
    public async Task<IActionResult> DeleteUserBookmark(int libraryId, int bookId, string clientId, CancellationToken token = default)
    {
        var deleteBookmarkCommand = new DeleteBookmarkRequest(libraryId, userHelper.AccountId ?? 0, bookId, clientId);
        await commandProcessor.SendAsync(deleteBookmarkCommand, cancellationToken: token);

        return NoContent();
    }

    [HttpGet("libraries/{libraryId}/my/books/{bookId}/notes", Name = nameof(GetUserNotes))]
    [Produces(typeof(IEnumerable<NoteView>))]
    public async Task<IActionResult> GetUserNotes(int libraryId, int bookId, CancellationToken token = default)
    {
        var query = new GetNotesQuery(libraryId, userHelper.AccountId ?? 0, bookId);
        var result = await queryProcessor.ExecuteAsync(query, token);

        return new OkObjectResult(result.Select(n => n.Map()));
    }

    [HttpPut("libraries/{libraryId}/my/books/{bookId}/notes/{clientId}", Name = nameof(UpsertUserNote))]
    [Produces(typeof(NoteView))]
    public async Task<IActionResult> UpsertUserNote(int libraryId,
        int bookId,
        string clientId,
        [FromBody] NoteView note,
        CancellationToken token = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var upsertNoteCommand = new UpsertNoteRequest(libraryId, userHelper.AccountId ?? 0, bookId, clientId, new NoteModel
        {
            ChapterId = note.ChapterId,
            StartOffset = note.StartOffset,
            EndOffset = note.EndOffset,
            Text = note.Text,
            Comment = note.Comment,
        });
        await commandProcessor.SendAsync(upsertNoteCommand, cancellationToken: token);

        if (upsertNoteCommand.Result?.Note == null)
        {
            return NotFound();
        }

        return new OkObjectResult(upsertNoteCommand.Result.Note.Map());
    }

    [HttpDelete("libraries/{libraryId}/my/books/{bookId}/notes/{clientId}", Name = nameof(DeleteUserNote))]
    public async Task<IActionResult> DeleteUserNote(int libraryId, int bookId, string clientId, CancellationToken token = default)
    {
        var deleteNoteCommand = new DeleteNoteRequest(libraryId, userHelper.AccountId ?? 0, bookId, clientId);
        await commandProcessor.SendAsync(deleteNoteCommand, cancellationToken: token);

        return NoContent();
    }
}
