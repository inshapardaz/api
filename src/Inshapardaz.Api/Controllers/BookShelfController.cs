using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library.Author;
using Inshapardaz.Domain.Ports.Handlers.Library.BookShelf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Controllers
{
    public class BookShelfController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderBookSelf _bookShelfRenderer;
        private readonly IRenderFile _fileRenderer;
        private readonly IUserHelper _userHelper;

        public BookShelfController(IAmACommandProcessor commandProcessor,
                                IQueryProcessor queryProcessor,
                                IRenderBookSelf bookShelfRenderer,
                                IRenderFile fileRenderer,
                                IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _bookShelfRenderer = bookShelfRenderer;
            _fileRenderer = fileRenderer;
            _userHelper = userHelper;
        }

        [HttpGet("libraries/{libraryId}/bookshelves", Name = nameof(BookShelfController.GetBookShelves))]
        public async Task<IActionResult> GetBookShelves(int libraryId, string query, int pageNumber = 1, int pageSize = 10, CancellationToken token = default(CancellationToken))
        {
            var bookShelvesQuery = new GetBookShelfQuery(libraryId, pageNumber, pageSize) { Query = query };
            var result = await _queryProcessor.ExecuteAsync(bookShelvesQuery, token);

            var args = new PageRendererArgs<BookShelfModel>
            {
                Page = result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
            };

            return new OkObjectResult(_bookShelfRenderer.Render(args, libraryId));
        }

        [HttpGet("libraries/{libraryId}/bookshelves/{bookShelfId}", Name = nameof(BookShelfController.GetBookShelf))]
        public async Task<IActionResult> GetBookShelf(int libraryId, int bookShelfId, CancellationToken token = default(CancellationToken))
        {
            var query = new GetBookShelfByIdQuery(libraryId, bookShelfId);
            var bookShelf = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (bookShelf != null)
            {
                return new OkObjectResult(_bookShelfRenderer.Render(bookShelf, libraryId));
            }

            return new NotFoundResult();
        }

        [HttpPost("libraries/{libraryId}/bookshelves", Name = nameof(BookShelfController.CreateBookShelf))]
        [Authorize()]
        public async Task<IActionResult> CreateBookShelf(int libraryId, [FromBody] BookShelfView bookShelf, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AddBookShelfRequest(libraryId, bookShelf.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _bookShelfRenderer.Render(request.Result, libraryId);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        [HttpPut("libraries/{libraryId}/bookshelves/{bookShelfId}", Name = nameof(BookShelfController.UpdateBookShelf))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateBookShelf(int libraryId, int bookShelfId, [FromBody] BookShelfView bookShelf, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid || bookShelfId != bookShelf.Id)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new UpdateBookShelfRequest(libraryId, bookShelf.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _bookShelfRenderer.Render(request.Result.BookShelf, libraryId);
            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }
            else
            {
                return new OkObjectResult(renderResult);
            }
        }

        [HttpDelete("libraries/{libraryId}/bookshelves/{bookShelfId}", Name = nameof(BookShelfController.DeleteBookShelf))]
        [Authorize(Role.Admin, Role.LibraryAdmin)]
        public async Task<IActionResult> DeleteBookShelf(int libraryId, int bookShelfId, CancellationToken token = default)
        {
            var request = new DeleteBookSelfRequest(libraryId, bookShelfId);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPut("libraries/{libraryId}/bookshelves/{bookShelfId}/image", Name = nameof(BookShelfController.UpdateBookShelfImage))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateBookShelfImage(int libraryId, int bookShelfId, [FromForm] IFormFile file, CancellationToken token = default)
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var request = new UpdateBookShelfImageRequest(libraryId, bookShelfId)
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

        [HttpPost("libraries/{libraryId}/bookshelves/{bookShelfId}", Name = nameof(BookShelfController.AddBookInBookShelf))]
        [Authorize()]
        public async Task<IActionResult> AddBookInBookShelf(int libraryId, int bookShelfId, [FromBody] BookShelfBookView bookShelfBook, CancellationToken token = default(CancellationToken))
        {
            var request = new AddBookToBookShelfRequest(libraryId, bookShelfId, 
                bookShelfBook.BookId, 
                bookShelfBook.Index, 
                _userHelper.Account.Id);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return Ok();
        }

        [HttpPut("libraries/{libraryId}/bookshelves/{bookShelfId}/books/{bookId}", Name = nameof(BookShelfController.UpdateBookInBookShelf))]
        [Authorize()]
        public async Task<IActionResult> UpdateBookInBookShelf(int libraryId, int bookShelfId, int booksId, [FromBody] BookShelfBookView bookShelfBook, CancellationToken token = default(CancellationToken))
        {
            var request = new UpdateBookToBookShelfRequest(libraryId, bookShelfId,
                booksId,
                bookShelfBook.Index,
                _userHelper.Account.Id);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return Ok();
        }

        [HttpDelete("libraries/{libraryId}/bookshelves/{bookShelfId}/books/{bookId}", Name = nameof(BookShelfController.DeleteBookInBookShelf))]
        [Authorize()]
        public async Task<IActionResult> DeleteBookInBookShelf(int libraryId, int bookShelfId, int bookId, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteBookFromBookShelfRequest(libraryId, bookShelfId,
                bookId,
                _userHelper.Account.Id);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }
    }
}
