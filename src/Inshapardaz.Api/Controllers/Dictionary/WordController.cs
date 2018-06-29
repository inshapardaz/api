using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Ports.Dictionary;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers.Dictionary
{
    public class WordController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderWord _wordRenderer;
        private readonly IRenderWordPage _wordPageRenderer;

        public WordController(IAmACommandProcessor commandProcessor, IRenderWord wordRenderer, IRenderWordPage wordPageRenderer)
        {
            _commandProcessor = commandProcessor;
            _wordRenderer = wordRenderer;
            _wordPageRenderer = wordPageRenderer;
        }

        [HttpGet("api/dictionaries/{id}/words", Name = "GetWords")]
        [Produces(typeof(PageView<WordView>))]
        public async Task<IActionResult> GetWords(int id, int pageNumber = 1, int pageSize = 10)
        {
            var command = new GetWordsForDictionaryRequest(id, pageNumber, pageSize);
            await _commandProcessor.SendAsync(command);

            var args = new PageRendererArgs<Word>
            {
                Page = command.Result,
                RouteArguments = new PagedRouteArgs { DictionaryId =  id, PageNumber = pageNumber, PageSize = pageSize},
                RouteName = "GetWords"
            };
            return Ok(_wordPageRenderer.Render(args, id));
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}", Name = "GetWordById")]
        [Produces(typeof(WordView))]
        public async Task<IActionResult> GetWord(int id, int wordId)
        {
            var command = new GetWordByIdRequest(id, wordId);
            await _commandProcessor.SendAsync(command);

            return Ok(_wordRenderer.Render(command.Result, id));
        }

        [HttpPost("/api/dictionaries/{id}/words", Name = "CreateWord")]
        [Produces(typeof(WordView))]
        [ValidateModel]
        public async Task<IActionResult> Post(int id, [FromBody] WordView word)
        {
            var command = new AddWordRequest(id, word.Map<WordView, Word>());
            await _commandProcessor.SendAsync(command);

            var response = _wordRenderer.Render(command.Result, id);
            return Created(response.Links.Self(), response);
        }

        [HttpPut("/api/dictionaries/{id}/words/{wordId}", Name = "UpdateWord")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, int wordId, [FromBody] WordView word)
        {
            var request = new UpdateWordRequest(id, word.Map<WordView, Word>() );
            await _commandProcessor.SendAsync(request);

            if (request.Result.HasAddedNew)
            {
                var response = _wordRenderer.Render(request.Result.Word, id);
                return Created(response.Links.Self(), response);
            }

            return NoContent();
        }

        [HttpDelete("/api/dictionaries/{id}/words/{wordId}", Name = "DeleteWord")]
        public async Task<IActionResult> Delete(int id, int wordId)
        {
            var command = new DeleteWordRequest(id) { WordId = wordId };
            await _commandProcessor.SendAsync(command);
            return NoContent();
        }
    }
}