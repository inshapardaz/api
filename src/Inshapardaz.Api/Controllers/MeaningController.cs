using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using System.Threading.Tasks;
using Inshapardaz.Api.Adapters.Dictionary;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.Controllers
{
    public class MeaningController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;

        public MeaningController(IAmACommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/meanings", Name = "GetWordMeaningByWordId")]
        public async Task<IActionResult> GetMeaningForWord(int id, int wordId)
        {
            var request = new GetMeaningForWordRequest
            {
                DictionaryId = id,
                WordId = wordId
            };

            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpGet("api/dictionaries/{id}/details/{detailId}/meanings", Name = "GetWordMeaningByWordDetailId")]
        public async Task<IActionResult> GetMeaningForWordDetail(int id, int detailId)
        {
            var request = new GetMeaningForWordDetailRequest
            {
                DictionaryId = id,
                DetailId = detailId
            };
            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpGet("api/dictionaries/{id}/meanings/{meaningId}", Name = "GetMeaningById")]
        public async Task<IActionResult> Get(int id, int meaningId)
        {
            var request = new GetMeaningByIdRequest
            {
                DictionaryId = id,
                MeaningId = meaningId
            };
            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/meanings/contexts/{context}", Name = "GetWordMeaningByContext")]
        public async Task<IActionResult> GetMeaningForContext(int id, int wordId, string context)
        {
            var request = new GetMeaningForContextRequest
            {
                DictionaryId = id,
                WordId = wordId,
                Context = context
            };
            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpPost("api/dictionaries/{id}/details/{detailId}/meanings", Name = "AddMeaning")]
        public async Task<IActionResult> Post(int id, int detailId, [FromBody]MeaningView meaning)
        {
            var request = new PostMeaningRequest
            {
                DictionaryId = id,
                DetailId = detailId,
                Meaning = meaning
            };
            await _commandProcessor.SendAsync(request);
            return Created(request.Result.Location, request.Result.Response);
        }

        [HttpPut("api/dictionaries/{id}/meanings/{meaningId}", Name = "UpdateMeaning")]
        public async Task<IActionResult> Put(int id, int meaningId, [FromBody]MeaningView meaning)
        {
            var request = new PutMeaningRequest
            {
                DictionaryId = id,
                MeaningId = meaningId,
                Meaning = meaning
            };
            await _commandProcessor.SendAsync(request);
            return NoContent();
        }

        [HttpDelete("api/dictionaries/{id}/meanings/{meaningId}", Name = "DeleteMeaning")]
        public async Task<IActionResult> Delete(int id, int meaningId)
        {
            var request = new DeleteMeaningRequest
            {
                DictionaryId = id,
                MeaningId = meaningId
            };
            await _commandProcessor.SendAsync(request);
            return NoContent();
        }
    }
}