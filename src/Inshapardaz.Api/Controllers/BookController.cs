using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Controllers
{
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

        [HttpGet("library/{libraryId}/books")]
        public async Task<IActionResult> GetBooks(int libraryId,
            string query,
            int pageNumber = 1,
            int pageSize = 10,
            [FromQuery]int authorId = 0,
            [FromQuery]int categoryId = 0,
            [FromQuery]int seriesId = 0,
            [FromQuery]bool? favorite = null,
            [FromQuery]bool? read = null,
            [FromQuery]BookSortByType sortBy = BookSortByType.Title,
            [FromQuery]SortDirection sortDirection = SortDirection.Ascending,
            CancellationToken token = default(CancellationToken))
        {
            var filter = new BookFilter
            {
                AuthorId = authorId,
                CategoryId = categoryId,
                SeriesId = seriesId,
                Favorite = favorite,
                Read = read
            };
            var request = new GetBooksQuery(libraryId, pageNumber, pageSize, _userHelper.GetUserId())
            {
                Query = query,
                Filter = filter,
                SortBy = sortBy,
                SortDirection = sortDirection
            };
            var books = await _queryProcessor.ExecuteAsync(request, cancellationToken: token);

            var args = new PageRendererArgs<BookModel>
            {
                Page = books,
                RouteArguments = new PagedRouteArgs
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    BookFilter = filter,
                    Query = query,
                    SortBy = sortBy,
                    SortDirection = sortDirection
                }
            };

            return new OkObjectResult(_bookRenderer.Render(args, libraryId));
        }

        [HttpGet("library/{libraryId}/books/{bookId}")]
        public async Task<IActionResult> GetBookById(int libraryId, int bookId, CancellationToken token)
        {
            var request = new GetBookByIdQuery(libraryId, bookId, _userHelper.GetUserId());
            var book = await _queryProcessor.ExecuteAsync(request, cancellationToken: token);

            if (book != null)
            {
                return new OkObjectResult(_bookRenderer.Render(book, libraryId));
            }

            return new NotFoundResult();
        }

        [HttpPost("library/{libraryId}/books")]
        [Authorize]
        public async Task<IActionResult> CreateBook(int libraryId, BookView book, CancellationToken token)
        {
            var request = new AddBookRequest(_userHelper.Claims, libraryId, book.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _bookRenderer.Render(request.Result, libraryId);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        [HttpPut("library/{libraryId}/books/{bookId}")]
        [Authorize]
        public async Task<IActionResult> UpdateBook(int libraryId, int bookId, BookView book, CancellationToken token)
        {
            book.Id = bookId;

            var request = new UpdateBookRequest(_userHelper.Claims, libraryId, book.Map());
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

        [HttpDelete("library/{libraryId}/books/{bookId}")]
        [Authorize]
        public async Task<IActionResult> DeleteBook(int libraryId, int bookId, CancellationToken token)
        {
            var request = new DeleteBookRequest(_userHelper.Claims, libraryId, bookId, _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPut("library/{libraryId}/books/{bookId}/image")]
        [Authorize]
        public async Task<IActionResult> UpdateBookImage(int libraryId, int bookId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var request = new UpdateBookImageRequest(_userHelper.Claims, libraryId, bookId)
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
                var response = _fileRenderer.Render(request.Result.File);

                return new CreatedResult(response.Links.Self(), response);
            }

            return new OkResult();
        }

        [HttpPost("library/{libraryId}/books/{bookId}/content")]
        [Authorize]
        public async Task<IActionResult> CreateBookContent(int libraryId, int bookId, int chapterId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }
            var language = Request.Headers["Accept-Language"];
            var mimeType = file.ContentType;

            var request = new AddBookContentRequest(_userHelper.Claims, libraryId, bookId, language, mimeType)
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

        [HttpPut("library/{libraryId}/books/{bookId}/content")]
        [Authorize]
        public async Task<IActionResult> UpdateBookContent(int libraryId, int bookId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }
            var language = Request.Headers["Accept-Language"];
            var mimeType = file.ContentType;

            var request = new UpdateBookContentRequest(_userHelper.Claims, libraryId, bookId, language, mimeType)
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

        [HttpDelete("library/{libraryId}/books/{bookId}/content")]
        [Authorize]
        public async Task<IActionResult> DeleteBookContent(int libraryId, int bookId, int chapterId, CancellationToken token = default(CancellationToken))
        {
            var mimeType = Request.Headers["Content-Type"];
            var language = Request.Headers["Accept-Language"];

            var request = new DeleteBookFileRequest(_userHelper.Claims, libraryId, bookId, language, mimeType, _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPost("library/{libraryId}/favorites/books/{bookId}")]
        [Authorize]
        public async Task<IActionResult> AddBookToFavorites(int libraryId, int bookId, CancellationToken token)
        {
            var request = new AddBookToFavoriteRequest(_userHelper.Claims, libraryId, bookId, _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            return new OkResult();
            //return new CreatedResult(new Uri(Link(libraryId, bookId, RelTypes.Self).Href), null);
        }

        [HttpDelete("library/{libraryId}/favorites/books/{bookId}")]
        [Authorize]
        public async Task<IActionResult> RemoveBookFromFavorites(int libraryId, int bookId, CancellationToken token)
        {
            var request = new DeleteBookToFavoriteRequest(_userHelper.Claims, libraryId, bookId, _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            return new OkResult();
        }
    }
}
