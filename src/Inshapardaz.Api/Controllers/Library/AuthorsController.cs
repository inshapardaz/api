using System.IO;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Library;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Ports.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers.Library
{
    public class AuthorsController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderAuthors _authorsRenderer;
        private readonly IRenderAuthor _authorRenderer;
        private readonly IRenderFile _fileRenderer;

        public AuthorsController(IAmACommandProcessor commandProcessor, IRenderAuthors authorsRenderer, IRenderAuthor authorRenderer, IRenderFile fileRenderer)
        {
            _commandProcessor = commandProcessor;
            _authorsRenderer = authorsRenderer;
            _authorRenderer = authorRenderer;
            _fileRenderer = fileRenderer;
        }

        [HttpGet("/api/authors", Name = "GetAuthors")]
        [Produces(typeof(PageView<AuthorView>))]
        public async Task<IActionResult> GetAuthors(int pageNumber = 1, int pageSize = 10)
        {
            var request = new GetAuthorsRequest(pageNumber, pageSize);
            await _commandProcessor.SendAsync(request);

            var args = new PageRendererArgs<Author>
            {
                Page = request.Result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                RouteName = "GetAuthors"
            };

            return Ok(_authorsRenderer.Render(args));
        }

        [HttpGet("/api/authors/{id}", Name = "GetAuthorById")]
        [Produces(typeof(AuthorView))]
        public async Task<IActionResult> GetAuthorsById(int id)
        {
            var request = new GetAuthorByIdRequest(id);
            await _commandProcessor.SendAsync(request);

            if (request.Result != null)
                return Ok(_authorRenderer.Render(request.Result));
            return NotFound();
        }

        [Authorize]
        [HttpPost("/api/authors", Name = "CreateAuthor")]
        [Produces(typeof(AuthorView))]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody]AuthorView value)
        {
            var request = new AddAuthorRequest(value.Map<AuthorView, Author>());
            await _commandProcessor.SendAsync(request);

            var response = _authorRenderer.Render(request.Result);
            return Created(response.Links.Self(), response);
        }

        [Authorize]
        [HttpPut("/api/authors/{id}", Name = "UpdateAuthor")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, [FromBody] AuthorView value)
        {
            var request = new UpdateAuthorRequest(value.Map<AuthorView, Author>());
            await _commandProcessor.SendAsync(request);

            if (request.Result.HasAddedNew)
            {
                var response = _authorRenderer.Render(request.Result.Author);
                return Created(response.Links.Self(), response);
            }

            return NoContent();
        }

        [Authorize]
        [HttpPost("/api/authors/{id}/image", Name = "UpdateAuthorImage")]
        [ValidateModel]
        public async Task<IActionResult> PutImage(int id, IFormFile file)
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var request = new UpdateAuthorImageRequest(id)
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

            return BadRequest();
        }

        [Authorize]
        [HttpDelete("/api/authors/{id}", Name = "DeleteAuthor")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = new DeleteAuthorRequest(id);
            await _commandProcessor.SendAsync(request);
            return NoContent();
        }

    }
}
