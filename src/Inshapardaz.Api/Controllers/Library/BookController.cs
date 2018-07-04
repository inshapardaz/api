using System;
using System.Collections.Generic;
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
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using ObjectMapper = Inshapardaz.Api.Helpers.ObjectMapper;

namespace Inshapardaz.Api.Controllers.Library
{
    public class BookController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderBooks _booksRenderer;
        private readonly IRenderBook _renderBook;

        public BookController(IAmACommandProcessor commandProcessor, IUserHelper userHelper, IRenderBooks booksRenderer, IRenderBook renderBook)
        {
            _commandProcessor = commandProcessor;
            _userHelper = userHelper;
            _booksRenderer = booksRenderer;
            _renderBook = renderBook;
        }

        [HttpGet("/api/books", Name = "GetBooks")]
        [Produces(typeof(IEnumerable<BookView>))]
        public async Task<IActionResult> GetBooks(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new GetBooksRequest(pageNumber, pageSize)
            {
                UserId = _userHelper.GetUserId()
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

        [HttpGet("/api/genre/{id}/books", Name = "GetBooksByGenre")]
        [Produces(typeof(IEnumerable<BookView>))]
        public async Task<IActionResult> GetBooksByGenre(int id, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
            var request = new GetBooksByAuthorRequest(id, pageNumber, pageSize) { UserId = _userHelper.GetUserId() };
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var args = new PageRendererArgs<Book>
            {
                Page = request.Result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                RouteName = "GetBooksByGenre"
            };

            return Ok(_booksRenderer.Render(args));
        }

        [HttpGet("/api/books/{id}", Name = "GetBookById")]
        [Produces(typeof(BookView))]
        public async Task<IActionResult> GetBooksById(int id, CancellationToken cancellationToken)
        {
            var request = new GetBookByIdRequest(id);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            return Ok(_renderBook.Render(request.Result));
        }

        [Authorize]
        [HttpPost("/api/books", Name = "CreateBook")]
        [Produces(typeof(BookView))]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody]BookView value, CancellationToken cancellationToken)
        {
            var request = new AddBookRequest(ObjectMapper.Map<BookView, Book>(value));
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var renderResult = _renderBook.Render(request.Result);
            return Created(renderResult.Links.Self(), renderResult);
        }

        [Authorize]
        [HttpPut("/api/books/{id}", Name = "UpdateBook")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, [FromBody] BookView value, CancellationToken cancellationToken)
        {
            var request = new UpdateBookRequest(ObjectMapper.Map<BookView, Book>(value));
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result.HasAddedNew)
            {
                var renderResult = _renderBook.Render(request.Result.Book);
                return Created(renderResult.Links.Self(), renderResult);
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
