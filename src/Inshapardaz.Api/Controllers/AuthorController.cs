﻿using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library.Author;
using Inshapardaz.Domain.Ports.Query.Library.Author;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

public class AuthorController : Controller
{
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IRenderAuthor _authorRenderer;
    private readonly IRenderFile _fileRenderer;

    public AuthorController(IAmACommandProcessor commandProcessor,
                            IQueryProcessor queryProcessor,
                            IRenderAuthor bookRenderer,
                            IRenderFile fileRenderer)
    {
        _commandProcessor = commandProcessor;
        _queryProcessor = queryProcessor;
        _authorRenderer = bookRenderer;
        _fileRenderer = fileRenderer;
    }

    // TODO : Add sorting
    [HttpGet("libraries/{libraryId}/authors", Name = nameof(AuthorController.GetAuthors))]
    public async Task<IActionResult> GetAuthors(int libraryId, 
        string query, 
        AuthorTypes? authorType = null,
        int pageNumber = 1, 
        int pageSize = 10, 
        [FromQuery] AuthorSortByType sortBy = AuthorSortByType.Name,
        [FromQuery] SortDirection sortDirection = SortDirection.Ascending,
        CancellationToken token = default(CancellationToken))
    {
        var authorsQuery = new GetAuthorsQuery(libraryId, pageNumber, pageSize)
        {
            Query = query, 
            AuthorType = authorType,
            SortBy = sortBy,
            SortDirection = sortDirection
        };
        var result = await _queryProcessor.ExecuteAsync(authorsQuery, token);

        var args = new PageRendererArgs<AuthorModel>
        {
            Page = result,
            RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
        };

        return new OkObjectResult(_authorRenderer.Render(args, libraryId));
    }

    [HttpGet("libraries/{libraryId}/authors/{authorId}", Name = nameof(AuthorController.GetAuthorById))]
    public async Task<IActionResult> GetAuthorById(int libraryId, int authorId, CancellationToken token = default(CancellationToken))
    {
        var query = new GetAuthorByIdQuery(libraryId, authorId);
        var author = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

        if (author != null)
        {
            return new OkObjectResult(_authorRenderer.Render(author, libraryId));
        }

        return new NotFoundResult();
    }

    [HttpPost("libraries/{libraryId}/authors", Name = nameof(AuthorController.CreateAuthor))]
    public async Task<IActionResult> CreateAuthor(int libraryId, [FromBody] AuthorView author, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new AddAuthorRequest(libraryId, author.Map());
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = _authorRenderer.Render(request.Result, libraryId);
        return new CreatedResult(renderResult.Links.Self(), renderResult);
    }

    [HttpPut("libraries/{libraryId}/authors/{authorId}", Name = nameof(AuthorController.UpdateAuthor))]
    public async Task<IActionResult> UpdateAuthor(int libraryId, int authorId, [FromBody] AuthorView author, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var authorModel = author.Map();
        authorModel.Id = authorId;
        var request = new UpdateAuthorRequest(libraryId, authorModel);
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = _authorRenderer.Render(request.Result.Author, libraryId);
        if (request.Result.HasAddedNew)
        {
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }
        else
        {
            return new OkObjectResult(renderResult);
        }
    }

    [HttpDelete("libraries/{libraryId}/authors/{authorId}", Name = nameof(AuthorController.DeleteAuthor))]
    public async Task<IActionResult> DeleteAuthor(int libraryId, int authorId, CancellationToken token = default(CancellationToken))
    {
        var request = new DeleteAuthorRequest(libraryId, authorId);
        await _commandProcessor.SendAsync(request, cancellationToken: token);
        return new NoContentResult();
    }

    [HttpPut("libraries/{libraryId}/authors/{authorId}/image", Name = nameof(AuthorController.UpdateAuthorImage))]
    public async Task<IActionResult> UpdateAuthorImage(int libraryId, int authorId, [FromForm] IFormFile file, CancellationToken token = default(CancellationToken))
    {
        var content = new byte[file.Length];
        using (var stream = new MemoryStream(content))
        {
            await file.CopyToAsync(stream);
        }

        var request = new UpdateAuthorImageRequest(libraryId, authorId)
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
}
