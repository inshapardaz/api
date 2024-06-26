﻿using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library.BookShelf;
using Inshapardaz.Domain.Ports.Query.Library.BookShelf;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

public class BookShelfController : Controller
{
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IRenderBookSelf _bookShelfRenderer;
    private readonly IRenderFile _fileRenderer;

    public BookShelfController(IAmACommandProcessor commandProcessor,
                            IQueryProcessor queryProcessor,
                            IRenderBookSelf bookShelfRenderer,
                            IRenderFile fileRenderer)
    {
        _commandProcessor = commandProcessor;
        _queryProcessor = queryProcessor;
        _bookShelfRenderer = bookShelfRenderer;
        _fileRenderer = fileRenderer;
    }

    [HttpGet("libraries/{libraryId}/bookshelves", Name = nameof(GetBookShelves))]
    public async Task<IActionResult> GetBookShelves(int libraryId, string query, bool onlyPublic = false, int pageNumber = 1, int pageSize = 10, CancellationToken token = default(CancellationToken))
    {
        var bookShelvesQuery = new GetBookShelfQuery(libraryId, pageNumber, pageSize)
        {
            Query = query,
            OnlyPublic = onlyPublic
        };
        var result = await _queryProcessor.ExecuteAsync(bookShelvesQuery, token);

        var args = new PageRendererArgs<BookShelfModel>
        {
            Page = result,
            RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
        };

        return new OkObjectResult(_bookShelfRenderer.Render(args, libraryId));
    }

    [HttpGet("libraries/{libraryId}/bookshelves/{bookShelfId}", Name = nameof(GetBookShelf))]
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

    [HttpPost("libraries/{libraryId}/bookshelves", Name = nameof(CreateBookShelf))]
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

    [HttpPut("libraries/{libraryId}/bookshelves/{bookShelfId}", Name = nameof(UpdateBookShelf))]
    public async Task<IActionResult> UpdateBookShelf(int libraryId, int bookShelfId, [FromBody] BookShelfView bookShelf, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        bookShelf.Id = bookShelfId;
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

    [HttpDelete("libraries/{libraryId}/bookshelves/{bookShelfId}", Name = nameof(DeleteBookShelf))]
    public async Task<IActionResult> DeleteBookShelf(int libraryId, int bookShelfId, CancellationToken token = default)
    {
        var request = new DeleteBookSelfRequest(libraryId, bookShelfId);
        await _commandProcessor.SendAsync(request, cancellationToken: token);
        return new NoContentResult();
    }

    [HttpPut("libraries/{libraryId}/bookshelves/{bookShelfId}/image", Name = nameof(UpdateBookShelfImage))]
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
            var response = _fileRenderer.Render(libraryId, request.Result.File);
            return new CreatedResult(response.Links.Self(), response);
        }

        return new OkResult();
    }

    [HttpPost("libraries/{libraryId}/bookshelves/{bookShelfId}/books", Name = nameof(AddBookInBookShelf))]
    public async Task<IActionResult> AddBookInBookShelf(int libraryId, int bookShelfId, [FromBody] BookShelfBookView bookShelfBook, CancellationToken token = default(CancellationToken))
    {
        var request = new AddBookToBookShelfRequest(libraryId, bookShelfId,
            bookShelfBook.BookId,
            bookShelfBook.Index);
        await _commandProcessor.SendAsync(request, cancellationToken: token);
        return Ok();
    }

    [HttpPut("libraries/{libraryId}/bookshelves/{bookShelfId}/books/{bookId}", Name = nameof(BookShelfController.UpdateBookInBookShelf))]
    public async Task<IActionResult> UpdateBookInBookShelf(int libraryId, int bookShelfId, int bookId, [FromBody] BookShelfBookView bookShelfBook, CancellationToken token = default(CancellationToken))
    {
        var request = new UpdateBookToBookShelfRequest(libraryId, bookShelfId,
            bookId,
            bookShelfBook.Index);
        await _commandProcessor.SendAsync(request, cancellationToken: token);
        return Ok();
    }

    [HttpDelete("libraries/{libraryId}/bookshelves/{bookShelfId}/books/{bookId}", Name = nameof(DeleteBookInBookShelf))]
    public async Task<IActionResult> DeleteBookInBookShelf(int libraryId, int bookShelfId, int bookId, CancellationToken token = default(CancellationToken))
    {
        var request = new DeleteBookFromBookShelfRequest(libraryId, bookShelfId, bookId);
        await _commandProcessor.SendAsync(request, cancellationToken: token);
        return new NoContentResult();
    }
}
