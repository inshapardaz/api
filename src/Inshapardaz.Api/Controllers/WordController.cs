using System.Threading.Tasks;
using Inshapardaz.Api.Adapters.Dictionary;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.View;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers
{
    public class WordController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;

        public WordController(IAmACommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        [HttpGet("api/dictionaries/{id}/words", Name = "GetWords")]
        [Produces(typeof(PageView<WordView>))]
        public async Task<IActionResult> GetWords(int id, int pageNumber = 1, int pageSize = 10)
        {
            var command = new GetWordsForDictionaryRequest(id){ PageNumber = pageNumber, PageSize = pageSize };
            await _commandProcessor.SendAsync(command);
            return Ok(command.Result);
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}", Name = "GetWordById")]
        [Produces(typeof(WordView))]
        public async Task<IActionResult> GetWord(int id, int wordId)
        {
            var command = new GetWordByIdRequest(id) { WordId = wordId };
            await _commandProcessor.SendAsync(command);
            return Ok(command.Result);
        }

        [HttpPost("/api/dictionaries/{id}/words", Name = "CreateWord")]
        [Produces(typeof(WordView))]
        [ValidateModel]
        public async Task<IActionResult> Post(int id, [FromBody] WordView word)
        {
            var command = new PostWordRequest(id) { Word = word };
            await _commandProcessor.SendAsync(command);
            return Created(command.Result.Location, command.Result.Response);
        }

        [HttpPut("/api/dictionaries/{id}/words/{wordId}", Name = "UpdateWord")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, int wordId, [FromBody] WordView word)
        {
            var command = new PutWordRequest(id) { WordId = wordId, Word = word };
            await _commandProcessor.SendAsync(command);
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