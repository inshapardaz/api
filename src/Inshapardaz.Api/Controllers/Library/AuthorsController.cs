using System.Threading.Tasks;
using Inshapardaz.Api.Adapters.Library;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers.Library
{
    public class AuthorsController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;

        public AuthorsController(IAmACommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        [HttpGet("/api/authors", Name = "GetAuthors")]
        [Produces(typeof(PageView<AuthorView>))]
        public async Task<IActionResult> GetAuthors(int pageNumber = 1, int pageSize = 1)
        {
            var request = new GetAuthorsRequest(pageNumber, pageSize);
            await _commandProcessor.SendAsync(request);

            return Ok(request.Result);
        }

        [HttpGet("/api/authors/{id}", Name = "GetAuthorById")]
        [Produces(typeof(AuthorView))]
        public async Task<IActionResult> GetAuthorsById(int id)
        {
            var request = new GetAuthorByIdRequest(id);
            await _commandProcessor.SendAsync(request);

            return Ok(request.Result);
        }

        [Authorize]
        [HttpPost("/api/authors", Name = "CreateAuthor")]
        [Produces(typeof(AuthorView))]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody]AuthorView value)
        {
            var request = new PostAuthorRequest { Author = value };
            await _commandProcessor.SendAsync(request);

            return Created(request.Result.Location, request.Result.Response);
        }

        [Authorize]
        [HttpPut("/api/authors/{id}", Name = "UpdateAuthor")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, [FromBody] AuthorView value)
        {
            var request = new PutAuthorRequest { Author = value };
            await _commandProcessor.SendAsync(request);

            if (request.Result.Response != null)
            {
                return Created(request.Result.Location, request.Result.Response);
            }

            return NoContent();
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
