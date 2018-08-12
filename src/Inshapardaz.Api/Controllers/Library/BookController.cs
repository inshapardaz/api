using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Library;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Ports.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers.Library
{
    public class BookController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderBooks _booksRenderer;
        private readonly IRenderBook _renderBook;
        private readonly IRenderFile _fileRenderer;

        public BookController(IAmACommandProcessor commandProcessor, IUserHelper userHelper, IRenderBooks booksRenderer, IRenderBook renderBook, IRenderFile fileRenderer)
        {
            _commandProcessor = commandProcessor;
            _userHelper = userHelper;
            _booksRenderer = booksRenderer;
            _renderBook = renderBook;
            _fileRenderer = fileRenderer;
        }

        [HttpGet("/api/books", Name = "GetBooks")]
        [Produces(typeof(IEnumerable<BookView>))]
        public async Task<IActionResult> GetBooks(string query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new GetBooksRequest(pageNumber, pageSize)
            {
                UserId = _userHelper.GetUserId(),
                Query = query
            };
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var args = new PageRendererArgs<Book>
            {
                Page = request.Result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                RouteName = "GetBooks"
            };

            return Ok(_booksRenderer.Render(args));
        }

        [HttpGet("/api/authors/{id}/books", Name = "GetBooksByAuthor")]
        [Produces(typeof(IEnumerable<BookView>))]
        public async Task<IActionResult> GetBooksByAuthor(int id, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new GetBooksByAuthorRequest(id, pageNumber, pageSize) {UserId = _userHelper.GetUserId() };
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var args = new PageRendererArgs<Book>
            {
                Page = request.Result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                RouteName = "GetBooksByAuthor"
            };

            return Ok(_booksRenderer.Render(args));
        }

        [HttpGet("/api/categories/{id}/books", Name = "GetBooksByCategory")]
        [Produces(typeof(IEnumerable<BookView>))]
        public async Task<IActionResult> GetBooksByCategory(int id, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new GetBooksByCategoryRequest(id, pageNumber, pageSize) { UserId = _userHelper.GetUserId() };
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var args = new PageRendererArgs<Book>
            {
                Page = request.Result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                RouteName = "GetBooksByCategory"
            };

            return Ok(_booksRenderer.Render(args));
        }

        [HttpGet("/api/books/{id}", Name = "GetBookById")]
        [Produces(typeof(BookView))]
        public async Task<IActionResult> GetBooksById(int id, CancellationToken cancellationToken)
        {
            var request = new GetBookByIdRequest(id);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result != null)
                return Ok(_renderBook.Render(request.Result));
            return NotFound();
        }

        [Authorize]
        [HttpPost("/api/books", Name = "CreateBook")]
        [Produces(typeof(BookView))]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody]BookView value, CancellationToken cancellationToken)
        {
            var request = new AddBookRequest(value.Map<BookView, Book>());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var renderResult = _renderBook.Render(request.Result);
            return Created(renderResult.Links.Self(), renderResult);
        }

        [Authorize]
        [HttpPut("/api/books/{id}", Name = "UpdateBook")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, [FromBody] BookView value, CancellationToken cancellationToken)
        {
            var request = new UpdateBookRequest(value.Map<BookView, Book>());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result.HasAddedNew)
            {
                var renderResult = _renderBook.Render(request.Result.Book);
                return Created(renderResult.Links.Self(), renderResult);
            }

            return NoContent();
        }

        [Authorize]
        [HttpPost("/api/books/{id}/image", Name = "UpdateBookImage")]
        [ValidateModel]
        public async Task<IActionResult> PutImage(int id, IFormFile file)
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var request = new UpdateBookImageRequest(id)
            {
                Image = new Domain.Entities.File
                {
                    FileName = file.FileName,
                    MimeType = file.ContentType,
                    Contents = content
                }
            };

            await _commandProcessor.SendAsync(request);


            if (request.Result.HasAddedNew)
            {
                var response = _fileRenderer.Render(request.Result.File);
                return Created(response.Links.Self(), response);
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("/api/books/{id}", Name = "DeleteBook")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var request = new DeleteBookRequest(id);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);
            return NoContent();
        }

    }
}
