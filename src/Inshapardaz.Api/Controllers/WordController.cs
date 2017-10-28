using System.Threading.Tasks;
using Inshapardaz.Api.Adapters.Dictionary;
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

        [HttpGet]
        [Route("api/dictionaries/{id}/words", Name = "GetWords")]
        public async Task<IActionResult> GetWords(int id, int pageNumber = 1, int pageSize = 10)
        {
            var command = new GetWordsRequest{ DictionaryId = id, PageNumber = pageNumber, PageSize = pageSize };
            await _commandProcessor.SendAsync(command);
            return Ok(command.Result);
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}", Name = "GetWordById")]
        public async Task<IActionResult> GetWord(int id, int wordId)
        {
            var command = new GetWordByIdRequest { DictionaryId = id, WordId = wordId };
            await _commandProcessor.SendAsync(command);
            return Ok(command.Result);
        }

        [HttpPost("/api/dictionaries/{id}/words", Name = "CreateWord")]
        public async Task<IActionResult> Post(int id, [FromBody] WordView word)
        {
            var command = new PostWordRequest { DictionaryId = id, Word = word };
            await _commandProcessor.SendAsync(command);
            return Created(command.Result.Location, command.Result.Response);
        }

        [HttpPut("/api/dictionaries/{id}/words/{wordId}", Name = "UpdateWord")]
        public async Task<IActionResult> Put(int id, int wordId, [FromBody] WordView word)
        {
            var command = new PutWordRequest { DictionaryId = id, WordId = wordId, Word = word };
            await _commandProcessor.SendAsync(command);
            return NoContent();
        }

        [HttpDelete("/api/dictionaries/{id}/words/{wordId}", Name = "DeleteWord")]
        public async Task<IActionResult> Delete(int id, int wordId)
        {
            var command = new DeleteWordRequest { DictionaryId = id, WordId = wordId };
            await _commandProcessor.SendAsync(command);
            return NoContent();
        }
    }
}