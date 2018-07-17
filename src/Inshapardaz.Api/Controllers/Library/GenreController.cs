using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers.Library;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Ports.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers.Library
{
    public class GenreController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderGenres _renderGenres;
        private readonly IRenderGenre _renderGenre;

        public GenreController(IAmACommandProcessor commandProcessor, IRenderGenres renderGenres, IRenderGenre renderGenre)
        {
            _commandProcessor = commandProcessor;
            _renderGenres = renderGenres;
            _renderGenre = renderGenre;
        }

        [HttpGet("/api/genres", Name = "GetGenres")]
        [Produces(typeof(IEnumerable<GenreView>))]
        public async Task<IActionResult> GetGenres(CancellationToken cancellationToken)
        {
            var request = new GetGenresRequest();
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            return Ok(_renderGenres.RenderResult(request.Result));
        }

        [HttpGet("/api/genres/{id}", Name = "GetGenreById")]
        [Produces(typeof(GenreView))]
        public async Task<IActionResult> GetGenresById(int id, CancellationToken cancellationToken)
        {
            var request = new GetGenreByIdRequest(id);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result != null)
                return Ok(_renderGenre.RenderResult(request.Result));
            return NotFound();
        }

        [Authorize]
        [HttpPost("/api/genres", Name = "CreateGenre")]
        [Produces(typeof(GenreView))]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody]GenreView value, CancellationToken cancellationToken)
        {
            var request = new AddGenreRequest(value.Map<GenreView, Genre>());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var renderResult = _renderGenre.RenderResult(request.Result);
            return Created(renderResult.Links.Self(), renderResult);
        }

        [Authorize]
        [HttpPut("/api/genres/{id}", Name = "UpdateGenre")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, [FromBody] GenreView value, CancellationToken cancellationToken)
        {
            var request = new UpdateGenreRequest(value.Map<GenreView, Genre>());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result.HasAddedNew)
            {
                var renderResult = _renderGenre.RenderResult(request.Result.Genre);
                return Created(renderResult.Links.Self(), renderResult);
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("/api/genres/{id}", Name = "DeleteGenre")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var request = new DeleteGenreRequest(id);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);
            return NoContent();
        }

    }
}
