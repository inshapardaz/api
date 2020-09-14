using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderAuthor _authorRenderer;
        private readonly IRenderFile _fileRenderer;
        private readonly IUserHelper _userHelper;

        public AuthorController(IAmACommandProcessor commandProcessor,
                                IQueryProcessor queryProcessor,
                                IRenderAuthor bookRenderer,
                                IRenderFile fileRenderer,
                                IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _authorRenderer = bookRenderer;
            _fileRenderer = fileRenderer;
            _userHelper = userHelper;
        }

        [HttpGet("library/{libraryId}/authors", Name = nameof(AuthorController.GetAuthors))]
        public async Task<IActionResult> GetAuthors(int libraryId, string query, int pageNumber = 1, int pageSize = 10, CancellationToken token = default(CancellationToken))
        {
            var authorsQuery = new GetAuthorsQuery(libraryId, pageNumber, pageSize) { Query = query };
            var result = await _queryProcessor.ExecuteAsync(authorsQuery, token);

            var args = new PageRendererArgs<AuthorModel>
            {
                Page = result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
            };

            return new OkObjectResult(_authorRenderer.Render(args, libraryId));
        }

        [HttpGet("library/{libraryId}/authors/{authorId}", Name = nameof(AuthorController.GetAuthorById))]
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

        [HttpPost("library/{libraryId}/authors", Name = nameof(AuthorController.CreateAuthor))]
        public async Task<IActionResult> CreateAuthor(int libraryId, [FromBody]AuthorView author, CancellationToken token = default(CancellationToken))
        {
            var request = new AddAuthorRequest(_userHelper.Claims, libraryId, author.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _authorRenderer.Render(request.Result, libraryId);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        [HttpPut("library/{libraryId}/authors/{authorId}", Name = nameof(AuthorController.UpdateAuthor))]
        public async Task<IActionResult> UpdateAuthor(int libraryId, int authorId, [FromBody]AuthorView author, CancellationToken token = default(CancellationToken))
        {
            var request = new UpdateAuthorRequest(_userHelper.Claims, libraryId, author.Map());
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

        [HttpDelete("library/{libraryId}/authors/{authorId}", Name = nameof(AuthorController.DeleteAuthor))]
        public async Task<IActionResult> DeleteAuthor(int libraryId, int authorId, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteAuthorRequest(_userHelper.Claims, libraryId, authorId);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPut("library/{libraryId}/authors/{authorId}/image", Name = nameof(AuthorController.UpdateAuthorImage))]
        public async Task<IActionResult> UpdateAuthorImage(int libraryId, int authorId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var request = new UpdateAuthorImageRequest(_userHelper.Claims, libraryId, authorId)
            {
                Image = new Domain.Models.FileModel
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
    }
}
