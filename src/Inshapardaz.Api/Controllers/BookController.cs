using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library.Book;
using Inshapardaz.Domain.Ports.Query.Library;
using Inshapardaz.Domain.Ports.Query.Library.Book;
using Inshapardaz.Domain.Ports.Query.Library.BookShelf;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

public class BookController(
    IAmACommandProcessor commandProcessor,
    IQueryProcessor queryProcessor,
    IRenderBook bookRenderer,
    IRenderFile fileRenderer,
    IUserHelper userHelper)
    : Controller
{
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
        [FromQuery] string language = null,
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
            AssignmentStatus = assignedFor,
            Language = language
        };

        BookShelfModel bookShelf = null;
        if (bookShelfId.HasValue)
        {
            bookShelf = await queryProcessor.ExecuteAsync(new GetBookShelfByIdQuery(libraryId, bookShelfId.Value), token);
        };
        var request = new GetBooksQuery(libraryId, pageNumber, pageSize, userHelper.AccountId)
        {
            Query = query,
            Filter = filter,
            SortBy = sortBy,
            SortDirection = sortDirection
        };

        var books = await queryProcessor.ExecuteAsync(request, cancellationToken: token);
        
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

        return new OkObjectResult(bookRenderer.Render(args, libraryId, bookShelf));
    }

    [HttpGet("libraries/{libraryId}/books/{bookId}", Name = nameof(BookController.GetBookById))]
    public async Task<IActionResult> GetBookById(int libraryId, int bookId, CancellationToken token)
    {
        var request = new GetBookByIdQuery(libraryId, bookId, userHelper.AccountId);
        var book = await queryProcessor.ExecuteAsync(request, cancellationToken: token);

        if (book != null)
        {
            return new OkObjectResult(bookRenderer.Render(book, libraryId));
        }

        return new NotFoundResult();
    }

    [HttpGet("libraries/{libraryId}/books/{bookId}/rating", Name = nameof(BookController.GetBookRating))]
    [Produces(typeof(RatingSummaryView))]
    public async Task<IActionResult> GetBookRating(int libraryId, int bookId, CancellationToken token)
    {
        var request = new GetBookRatingSummaryQuery(libraryId, bookId, userHelper.AccountId);
        var summary = await queryProcessor.ExecuteAsync(request, cancellationToken: token);

        if (summary == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(summary.Map());
    }

    [HttpPost("libraries/{libraryId}/books", Name = nameof(BookController.CreateBook))]
    public async Task<IActionResult> CreateBook(int libraryId, [FromBody] BookView book, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new AddBookRequest(libraryId, userHelper.AccountId, book.Map());
        await commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = bookRenderer.Render(request.Result, libraryId);
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

        var request = new UpdateBookRequest(libraryId, userHelper.AccountId, book.Map());
        await commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = bookRenderer.Render(request.Result.Book, libraryId);

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
        var request = new DeleteBookRequest(libraryId, bookId, userHelper.AccountId);
        await commandProcessor.SendAsync(request, cancellationToken: token);
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

        var request = new UpdateBookImageRequest(libraryId, bookId, userHelper.AccountId)
        {
            Image = new FileModel
            {
                FileName = file.FileName,
                MimeType = file.ContentType,
                Contents = content
            }
        };

        await commandProcessor.SendAsync(request, cancellationToken: token);

        if (request.Result.HasAddedNew)
        {
            var response = fileRenderer.Render(libraryId, request.Result.File);

            return new CreatedResult(response.Links.Self(), response);
        }

        return new OkResult();
    }

    [HttpGet("libraries/{libraryId}/books/{bookId}/contents/{contentId}", Name = nameof(BookController.GetBookContent))]
    public async Task<IActionResult> GetBookContent(int libraryId, int bookId, int contentId, [FromQuery] string language, CancellationToken token = default(CancellationToken))
    {
        var mimeType = Request.Headers["Accept"];

        var request = new GetBookContentQuery(libraryId, bookId, contentId, language, mimeType, userHelper.AccountId);
        var content = await queryProcessor.ExecuteAsync(request, cancellationToken: token);
        if (content != null)
        {
            return new OkObjectResult(bookRenderer.Render(content, libraryId));
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

        var request = new AddBookContentRequest(libraryId, bookId, language, mimeType, userHelper.AccountId)
        {
            Content = new FileModel
            {
                Contents = content,
                MimeType = mimeType,
                DateCreated = DateTime.Now,
                FileName = file.FileName
            }
        };

        await commandProcessor.SendAsync(request, cancellationToken: token);

        if (request.Result != null)
        {
            var response = bookRenderer.Render(request.Result, libraryId);
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

        var request = new UpdateBookContentRequest(libraryId, bookId, contentId, language, mimeType, userHelper.AccountId)
        {
            Content = new FileModel
            {
                Contents = content,
                MimeType = mimeType,
                DateCreated = DateTime.Now,
                FileName = file.FileName
            }
        };

        await commandProcessor.SendAsync(request, cancellationToken: token);

        if (request.Result.Content != null)
        {
            var renderResult = bookRenderer.Render(request.Result.Content, libraryId);

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
        await commandProcessor.SendAsync(request, cancellationToken: token);
        return new NoContentResult();
    }

    [HttpPost("libraries/{libraryId}/favorites/books/{bookId}", Name = nameof(BookController.AddBookToFavorites))]
    public async Task<IActionResult> AddBookToFavorites(int libraryId, int bookId, CancellationToken token)
    {
        var request = new AddBookToFavoriteRequest(libraryId, bookId, userHelper.AccountId);
        await commandProcessor.SendAsync(request, cancellationToken: token);

        return new OkResult();
    }

    [HttpDelete("libraries/{libraryId}/favorites/books/{bookId}", Name = nameof(BookController.RemoveBookFromFavorites))]
    public async Task<IActionResult> RemoveBookFromFavorites(int libraryId, int bookId, CancellationToken token)
    {
        var request = new DeleteBookFromFavoriteRequest(libraryId, bookId, userHelper.AccountId);
        await commandProcessor.SendAsync(request, cancellationToken: token);

        return new OkResult();
    }

    [HttpGet("libraries/{libraryId}/books/{bookId}/bind", Name = nameof(BindBook))]
    public async Task<IActionResult> BindBook(int libraryId, int bookId, CancellationToken token)
    {
        var request = new BindBookRequest(libraryId, bookId);
        await commandProcessor.SendAsync(request, cancellationToken: token);

        return File(System.Text.Encoding.UTF8.GetBytes(request.Result), MimeTypes.Text);
    }

    [HttpPost("libraries/{libraryId}/books/{bookId}/publish", Name = nameof(PublishBook))]
    public async Task<IActionResult> PublishBook(int libraryId, int bookId, [FromBody] PublishBookRequestView publishBookRequest, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }
        
        var request = new PublishBookRequest(libraryId, bookId)
        {
            OutputType = publishBookRequest.OutputType,
            OnlyPublishFile = publishBookRequest.OnlyPublishFile,
        };
        await commandProcessor.SendAsync(request, cancellationToken: token);

        return Ok();
    }
    
    [HttpGet("libraries/{libraryId}/publishers", Name = nameof(GetPublishers))]
    public async Task<IActionResult> GetPublishers(
        int libraryId, 
        string query,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken token = default)
    {
        var request = new GetPublishersQuery(libraryId, query, pageNumber, pageSize);
        var result = await queryProcessor.ExecuteAsync(request, cancellationToken: token);

        return Ok(bookRenderer.Render(libraryId, new PageRendererArgs<string>
            {
                Page = result,
                RouteArguments = new PagedRouteArgs
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Query = query
                }
            }));
    }

}
