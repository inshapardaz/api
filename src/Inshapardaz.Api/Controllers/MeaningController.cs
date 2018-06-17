using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using System.Threading.Tasks;
using Inshapardaz.Api.Adapters.Dictionary;
using Inshapardaz.Api.Middlewares;
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
        [Produces(typeof(IEnumerable<MeaningView>))]
        public async Task<IActionResult> GetMeaningForWord(int id, int wordId)
        {
            var request = new GetMeaningForWordRequest(id) { WordId = wordId };

            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/meanings/{meaningId}", Name = "GetMeaningById")]
        [Produces(typeof(MeaningView))]
        public async Task<IActionResult> Get(int id, long wordId, long meaningId)
        {
            var request = new GetMeaningByIdRequest(id, wordId, meaningId);
            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/meanings/contexts/{context}", Name = "GetWordMeaningByContext")]
        [Produces(typeof(IEnumerable<MeaningView>))]
        public async Task<IActionResult> GetMeaningsForContext(int id, long wordId, string context)
        {
            var request = new GetMeaningForContextRequest(id, wordId, context);
            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpPost("api/dictionaries/{id}/words/{wordId}/meanings", Name = "AddMeaning")]
        [Produces(typeof(MeaningView))]
        [ValidateModel]
        public async Task<IActionResult> Post(int id, int wordId, [FromBody]MeaningView meaning)
        {
            var request = new PostMeaningRequest(id)
            {
                WordId = wordId,
                Meaning = meaning
            };
            await _commandProcessor.SendAsync(request);
            return Created(request.Result.Location, request.Result.Response);
        }

        [HttpPut("api/dictionaries/{id}/words/{wordId}/meanings/{meaningId}", Name = "UpdateMeaning")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, long wordId, int meaningId, [FromBody]MeaningView meaning)
        {
            var request = new PutMeaningRequest(id, wordId, meaningId)
            {
                Meaning = meaning
            };
            await _commandProcessor.SendAsync(request);
            return NoContent();
        }

        [HttpDelete("api/dictionaries/{id}/words/{wordId}/meanings/{meaningId}", Name = "DeleteMeaning")]
        public async Task<IActionResult> Delete(int id, int wordId, int meaningId)
        {
            var request = new DeleteMeaningRequest(id, wordId, meaningId);
            await _commandProcessor.SendAsync(request);
            return NoContent();
        }
    }
}