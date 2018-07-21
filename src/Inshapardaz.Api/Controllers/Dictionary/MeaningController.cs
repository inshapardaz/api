using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Ports.Dictionary;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers.Dictionary
{
    public class MeaningController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderMeaning _meaningRenderer;

        public MeaningController(IAmACommandProcessor commandProcessor, IRenderMeaning meaningRenderer)
        {
            _commandProcessor = commandProcessor;
            _meaningRenderer = meaningRenderer;
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/meanings", Name = "GetWordMeaningByWordId")]
        [Produces(typeof(IEnumerable<MeaningView>))]
        public async Task<IActionResult> GetMeaningForWord(int id, int wordId)
        {
            var request = new GetMeaningForWordRequest(id, wordId);

            await _commandProcessor.SendAsync(request);
            return Ok(request.Result.Select(m => _meaningRenderer.Render(m, id)));
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/meanings/{meaningId}", Name = "GetMeaningById")]
        [Produces(typeof(MeaningView))]
        public async Task<IActionResult> Get(int id, long wordId, long meaningId)
        {
            var request = new GetMeaningByIdRequest(id, wordId, meaningId);
            await _commandProcessor.SendAsync(request);

            return Ok(_meaningRenderer.Render(request.Result, id));
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/meanings/contexts/{context}", Name = "GetWordMeaningByContext")]
        [Produces(typeof(IEnumerable<MeaningView>))]
        public async Task<IActionResult> GetMeaningsForContext(int id, long wordId, string context)
        {
            var request = new GetMeaningForContextRequest(id, wordId, context);
            await _commandProcessor.SendAsync(request);
            return Ok(request.Result.Select(m => _meaningRenderer.Render(m, id)));
        }

        [HttpPost("api/dictionaries/{id}/words/{wordId}/meanings", Name = "AddMeaning")]
        [Produces(typeof(MeaningView))]
        [ValidateModel]
        public async Task<IActionResult> Post(int id, int wordId, [FromBody]MeaningView meaning)
        {
            var request = new AddMeaningRequest(id, wordId, meaning.Map<MeaningView, Meaning>());
            await _commandProcessor.SendAsync(request);
            var response = _meaningRenderer.Render(request.Result, id);
            return Created(response.Links.Self(), response);
        }

        [HttpPut("api/dictionaries/{id}/words/{wordId}/meanings/{meaningId}", Name = "UpdateMeaning")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, long wordId, int meaningId, [FromBody]MeaningView meaning)
        {
            var request = new UpdateMeaningRequest(id, wordId, meaning.Map<MeaningView, Meaning>());
            await _commandProcessor.SendAsync(request);
            if (request.Result.HasAddedNew)
            {
                var response = _meaningRenderer.Render(request.Result.Meaning, id);
                return Created(response.Links.Self(), response);
            }

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