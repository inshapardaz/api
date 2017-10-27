using System.Threading.Tasks;
using Inshapardaz.Api.Ports;
using Inshapardaz.Api.View;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers
{
    public class WordDetailController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;

        public WordDetailController(IAmACommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        [HttpGet("/api/dictionaries/{id}/words/{wordId}/details", Name = "GetWordDetailsById")]
        public async Task<IActionResult> GetDetailForWord(int id, int wordId)
        {
            var request = new GetDetailsByWordIdRequest
            {
                DictionaryId = id,
                WordId = wordId
            };

            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpGet("/api/dictionaries/{id}/details/{detailId}", Name = "GetDetailsById")]
        public async Task<IActionResult> GetWordDetailById(int id, int detailId)
        {
            var request = new GetWordDetailByIdRequest
            {
                DictionaryId = id,
                DetailId = detailId
            };

            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpPost("/api/dictionaries/{id}/words/{wordId}/details", Name = "AddWordDetail")]
        public async Task<IActionResult> Post(int id, int wordId, [FromBody]WordDetailView wordDetail)
        {
            var request = new PostWordDetailRequest
            {
                DictionaryId = id,
                WordId = wordId,
                WordDetail = wordDetail
            };

            await _commandProcessor.SendAsync(request);
            return Created(request.Result.Location , request.Result.Response);
        }
        
        [HttpPut("/api/dictionaries/{id}/details/{detailId}", Name = "UpdateWordDetail")]
        public async Task<IActionResult> Put(int id, int detailId, [FromBody]WordDetailView wordDetail)
        {
            var request = new PutWordDetailRequest
            {
                DictionaryId = id,
                DetailId = detailId,
                WordDetail = wordDetail
            };

            await _commandProcessor.SendAsync(request);

            return NoContent();
        }

        [HttpDelete("/api/dictionaries/{id}/details/{detailId}", Name = "DeleteWordDetail")]
        public async Task<IActionResult> Delete(int id, int detailId)
        {
            var request = new DeleteWordDetailRequest
            {
                DictionaryId = id,
                DetailId = detailId
            };

            await _commandProcessor.SendAsync(request);

            return NoContent();
        }
    }
}