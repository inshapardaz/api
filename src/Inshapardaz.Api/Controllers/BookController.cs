using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
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

        [HttpGet("library/{libraryId}/books", Name = nameof(BookController.GetBooks))]
        public async Task<IActionResult> GetBooks(int libraryId,
            string query,
            int pageNumber = 1,
            int pageSize = 10,
            [FromQuery]int? authorId = null,
            [FromQuery]int? categoryId = null,
            [FromQuery]int? seriesId = null,
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
            var request = new GetBooksQuery(libraryId, pageNumber, pageSize, _userHelper.GetAccountId())
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

        [HttpGet("library/{libraryId}/books/{bookId}", Name = nameof(BookController.GetBookById))]
        public async Task<IActionResult> GetBookById(int libraryId, int bookId, CancellationToken token)
        {
            var request = new GetBookByIdQuery(libraryId, bookId, _userHelper.GetAccountId());
            var book = await _queryProcessor.ExecuteAsync(request, cancellationToken: token);

            if (book != null)
            {
                return new OkObjectResult(_bookRenderer.Render(book, libraryId));
            }

            return new NotFoundResult();
        }

        [HttpPost("library/{libraryId}/books", Name = nameof(BookController.CreateBook))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> CreateBook(int libraryId, [FromBody]BookView book, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AddBookRequest(libraryId, _userHelper.GetAccountId(), book.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _bookRenderer.Render(request.Result, libraryId);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        [HttpPut("library/{libraryId}/books/{bookId}", Name = nameof(BookController.UpdateBook))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateBook(int libraryId, int bookId, [FromBody]BookView book, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            book.Id = bookId;

            var request = new UpdateBookRequest(libraryId, _userHelper.GetAccountId(), book.Map());
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

        [HttpDelete("library/{libraryId}/books/{bookId}", Name = nameof(BookController.DeleteBook))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeleteBook(int libraryId, int bookId, CancellationToken token)
        {
            var request = new DeleteBookRequest(libraryId, bookId, _userHelper.GetAccountId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPut("library/{libraryId}/books/{bookId}/image", Name = nameof(BookController.UpdateBookImage))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateBookImage(int libraryId, int bookId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var request = new UpdateBookImageRequest(libraryId, bookId, _userHelper.GetAccountId())
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

        [HttpGet("library/{libraryId}/books/{bookId}/contents", Name = nameof(BookController.GetBookContent))]
        public async Task<IActionResult> GetBookContent(int libraryId, int bookId, CancellationToken token = default(CancellationToken))
        {
            var mimeType = Request.Headers["Accept"];
            var language = Request.Headers["Accept-Language"];

            var request = new GetBookContentQuery(libraryId, bookId, language, mimeType, _userHelper.GetAccountId());
            var content = await _queryProcessor.ExecuteAsync(request, cancellationToken: token);
            if (content != null)
            {
                return new OkObjectResult(_bookRenderer.Render(content, libraryId));
            }

            return new NotFoundResult();
        }

        [HttpPost("library/{libraryId}/books/{bookId}/contents", Name = nameof(BookController.CreateBookContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> CreateBookContent(int libraryId, int bookId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }
            var language = Request.Headers["Accept-Language"];
            var mimeType = file.ContentType;

            var request = new AddBookContentRequest(libraryId, bookId, language, mimeType, _userHelper.GetAccountId())
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

        [HttpPut("library/{libraryId}/books/{bookId}/contents", Name = nameof(BookController.UpdateBookContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateBookContent(int libraryId, int bookId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }
            var language = Request.Headers["Accept-Language"];
            var mimeType = file.ContentType;

            var request = new UpdateBookContentRequest(libraryId, bookId, language, mimeType, _userHelper.GetAccountId())
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

        [HttpDelete("library/{libraryId}/books/{bookId}/contents", Name = nameof(BookController.DeleteBookContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeleteBookContent(int libraryId, int bookId, CancellationToken token = default(CancellationToken))
        {
            var mimeType = Request.Headers["Accept"];
            var language = Request.Headers["Accept-Language"];

            var request = new DeleteBookContentRequest(libraryId, bookId, language, mimeType);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPost("library/{libraryId}/favorites/books/{bookId}", Name = nameof(BookController.AddBookToFavorites))]
        [Authorize]
        public async Task<IActionResult> AddBookToFavorites(int libraryId, int bookId, CancellationToken token)
        {
            var request = new AddBookToFavoriteRequest(libraryId, bookId, _userHelper.GetAccountId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            return new OkResult();
        }

        [HttpDelete("library/{libraryId}/favorites/books/{bookId}", Name = nameof(BookController.RemoveBookFromFavorites))]
        [Authorize]
        public async Task<IActionResult> RemoveBookFromFavorites(int libraryId, int bookId, CancellationToken token)
        {
            var request = new DeleteBookToFavoriteRequest(libraryId, bookId, _userHelper.GetAccountId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            return new OkResult();
        }
    }
}
