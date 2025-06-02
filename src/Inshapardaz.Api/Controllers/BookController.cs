using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library.Book;
using Inshapardaz.Domain.Ports.Query.Library.Book;
using Inshapardaz.Domain.Ports.Query.Library.BookShelf;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

public class BookController : Controller
{
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IRenderBook _bookRenderer;
    private readonly IRenderFile _fileRenderer;
    private readonly IUserHelper _userHelper;

    public BookController(IAmACommandProcessor commandProcessor,
        IQueryProcessor queryProcessor,
        IRenderBook bookRenderer,
        IRenderFile fileRenderer,
        IUserHelper userHelper)
    {
        _commandProcessor = commandProcessor;
        _queryProcessor = queryProcessor;
        _bookRenderer = bookRenderer;
        _fileRenderer = fileRenderer;
        _userHelper = userHelper;
    }

    [HttpGet("libraries/{libraryId}/books", Name = nameof(BookController.GetBooks))]
    public async Task<IActionResult> GetBooks(int libraryId,
        string query,
        int pageNumber = 1,
        int pageSize = 10,
        [FromQuery] int? authorId = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] int? tagId = null,
        [FromQuery] int? seriesId = null,
        [FromQuery] int? bookShelfId = null,
        [FromQuery] bool? favorite = null,
        [FromQuery] bool? read = null,
        [FromQuery] StatusType status = StatusType.Unknown,
        [FromQuery] BookSortByType sortBy = BookSortByType.Title,
        [FromQuery] SortDirection sortDirection = SortDirection.Ascending,
        [FromQuery] AssignmentStatus assignedFor = AssignmentStatus.None,
        CancellationToken token = default(CancellationToken))
    {
        var filter = new BookFilter
        {
            AuthorId = authorId,
            CategoryId = categoryId,
            TagId = tagId,
            SeriesId = seriesId,
            BookShelfId = bookShelfId,
            Favorite = favorite,
            Read = read,
            Status = status,
            AssignmentStatus = assignedFor
        };

        BookShelfModel bookShelf = null;
        if (bookShelfId.HasValue)
        {
            bookShelf = await _queryProcessor.ExecuteAsync(new GetBookShelfByIdQuery(libraryId, bookShelfId.Value), token);
        };
        var request = new GetBooksQuery(libraryId, pageNumber, pageSize, _userHelper.AccountId)
        {
            Query = query,
            Filter = filter,
            SortBy = sortBy,
            SortDirection = sortDirection
        };

        var books = await _queryProcessor.ExecuteAsync(request, cancellationToken: token);
        
        var args = new PageRendererArgs<BookModel, BookFilter>
        {
            Page = books,
            RouteArguments = new PagedRouteArgs
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Query = query,
                SortBy = sortBy,
                SortDirection = sortDirection
            },
            Filters = filter,
        };

        return new OkObjectResult(_bookRenderer.Render(args, libraryId, bookShelf));
    }

    [HttpGet("libraries/{libraryId}/books/{bookId}", Name = nameof(BookController.GetBookById))]
    public async Task<IActionResult> GetBookById(int libraryId, int bookId, CancellationToken token)
    {
        var request = new GetBookByIdQuery(libraryId, bookId, _userHelper.AccountId);
        var book = await _queryProcessor.ExecuteAsync(request, cancellationToken: token);

        if (book != null)
        {
            return new OkObjectResult(_bookRenderer.Render(book, libraryId));
        }

        return new NotFoundResult();
    }

    [HttpPost("libraries/{libraryId}/books", Name = nameof(BookController.CreateBook))]
    public async Task<IActionResult> CreateBook(int libraryId, [FromBody] BookView book, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new AddBookRequest(libraryId, _userHelper.AccountId, book.Map());
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = _bookRenderer.Render(request.Result, libraryId);
        return new CreatedResult(renderResult.Links.Self(), renderResult);
    }

    [HttpPut("libraries/{libraryId}/books/{bookId}", Name = nameof(BookController.UpdateBook))]
    public async Task<IActionResult> UpdateBook(int libraryId, int bookId, [FromBody] BookView book, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        book.Id = bookId;

        var request = new UpdateBookRequest(libraryId, _userHelper.AccountId, book.Map());
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = _bookRenderer.Render(request.Result.Book, libraryId);

        if (request.Result.HasAddedNew)
        {
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }
        else
        {
            return new OkObjectResult(renderResult);
        }
    }

    [HttpDelete("libraries/{libraryId}/books/{bookId}", Name = nameof(BookController.DeleteBook))]
    public async Task<IActionResult> DeleteBook(int libraryId, int bookId, CancellationToken token)
    {
        var request = new DeleteBookRequest(libraryId, bookId, _userHelper.AccountId);
        await _commandProcessor.SendAsync(request, cancellationToken: token);
        return new NoContentResult();
    }

    [HttpPut("libraries/{libraryId}/books/{bookId}/image", Name = nameof(BookController.UpdateBookImage))]
    public async Task<IActionResult> UpdateBookImage(int libraryId, int bookId, IFormFile file, CancellationToken token = default(CancellationToken))
    {
        var content = new byte[file.Length];
        using (var stream = new MemoryStream(content))
        {
            await file.CopyToAsync(stream);
        }

        var request = new UpdateBookImageRequest(libraryId, bookId, _userHelper.AccountId)
        {
            Image = new FileModel
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

    [HttpGet("libraries/{libraryId}/books/{bookId}/contents/{contentId}", Name = nameof(BookController.GetBookContent))]
    public async Task<IActionResult> GetBookContent(int libraryId, int bookId, int contentId, [FromQuery] string language, CancellationToken token = default(CancellationToken))
    {
        var mimeType = Request.Headers["Accept"];

        var request = new GetBookContentQuery(libraryId, bookId, contentId, language, mimeType, _userHelper.AccountId);
        var content = await _queryProcessor.ExecuteAsync(request, cancellationToken: token);
        if (content != null)
        {
            return new OkObjectResult(_bookRenderer.Render(content, libraryId));
        }

        return new NotFoundResult();
    }

    [HttpPost("libraries/{libraryId}/books/{bookId}/contents", Name = nameof(BookController.CreateBookContent))]
    public async Task<IActionResult> CreateBookContent(int libraryId, int bookId, [FromQuery] string language, IFormFile file, CancellationToken token = default(CancellationToken))
    {
        var content = new byte[file.Length];
        using (var stream = new MemoryStream(content))
        {
            await file.CopyToAsync(stream);
        }

        var mimeType = file.ContentType;

        var request = new AddBookContentRequest(libraryId, bookId, language, mimeType, _userHelper.AccountId)
        {
            Content = new FileModel
            {
                Contents = content,
                MimeType = mimeType,
                DateCreated = DateTime.Now,
                FileName = file.FileName
            }
        };

        await _commandProcessor.SendAsync(request, cancellationToken: token);

        if (request.Result != null)
        {
            var response = _bookRenderer.Render(request.Result, libraryId);
            return new CreatedResult(response.Links.Self(), response);
        }

        return new BadRequestResult();
    }

    [HttpPut("libraries/{libraryId}/books/{bookId}/contents/{contentId}", Name = nameof(BookController.UpdateBookContent))]
    public async Task<IActionResult> UpdateBookContent(int libraryId, int bookId, int contentId, [FromQuery] string language, IFormFile file, CancellationToken token = default(CancellationToken))
    {
        var content = new byte[file.Length];
        using (var stream = new MemoryStream(content))
        {
            await file.CopyToAsync(stream);
        }

        var mimeType = file.ContentType;

        var request = new UpdateBookContentRequest(libraryId, bookId, contentId, language, mimeType, _userHelper.AccountId)
        {
            Content = new FileModel
            {
                Contents = content,
                MimeType = mimeType,
                DateCreated = DateTime.Now,
                FileName = file.FileName
            }
        };

        await _commandProcessor.SendAsync(request, cancellationToken: token);

        if (request.Result.Content != null)
        {
            var renderResult = _bookRenderer.Render(request.Result.Content, libraryId);

            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }
            else
            {
                return new OkObjectResult(renderResult);
            }
        }

        return new BadRequestResult();
    }

    [HttpDelete("libraries/{libraryId}/books/{bookId}/contents/{contentId}", Name = nameof(BookController.DeleteBookContent))]
    public async Task<IActionResult> DeleteBookContent(int libraryId, int bookId, int contentId, CancellationToken token = default(CancellationToken))
    {
        var request = new DeleteBookContentRequest(libraryId, bookId, contentId);
        await _commandProcessor.SendAsync(request, cancellationToken: token);
        return new NoContentResult();
    }

    [HttpPost("libraries/{libraryId}/favorites/books/{bookId}", Name = nameof(BookController.AddBookToFavorites))]
    public async Task<IActionResult> AddBookToFavorites(int libraryId, int bookId, CancellationToken token)
    {
        var request = new AddBookToFavoriteRequest(libraryId, bookId, _userHelper.AccountId);
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        return new OkResult();
    }

    [HttpDelete("libraries/{libraryId}/favorites/books/{bookId}", Name = nameof(BookController.RemoveBookFromFavorites))]
    public async Task<IActionResult> RemoveBookFromFavorites(int libraryId, int bookId, CancellationToken token)
    {
        var request = new DeleteBookFromFavoriteRequest(libraryId, bookId, _userHelper.AccountId);
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        return new OkResult();
    }

    [HttpGet("libraries/{libraryId}/books/{bookId}/bind", Name = nameof(BindBook))]
    public async Task<IActionResult> BindBook(int libraryId, int bookId, CancellationToken token)
    {
        var request = new BindBookRequest(libraryId, bookId);
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        return File(System.Text.Encoding.UTF8.GetBytes(request.Result), MimeTypes.Text);
    }

    [HttpPost("libraries/{libraryId}/books/{bookId}/publish", Name = nameof(PublishBook))]
    public async Task<IActionResult> PublishBook(int libraryId, int bookId, CancellationToken token)
    {
        var request = new PublishBookRequest(libraryId, bookId);
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        return Ok();
    }
}
