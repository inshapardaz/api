using System.Collections.Generic;
using System.Threading.Tasks;
using Inshapardaz.Api.Adapters.Dictionary;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Ports.Dictionary;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers.Dictionary
{
    public class TranslationController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;

        public TranslationController(IAmACommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/translations", Name = "GetWordTranslationsById")]
        [Produces(typeof(IEnumerable<TranslationView>))]
        public async Task<IActionResult> GetTranslationForWord(int id, int wordId)
        {
            var request = new GetTranslationForWordRequest(id)
            {
                WordId = wordId
            };

            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/translations/languages/{language}", Name = "GetWordTranslationsByWordIdAndLanguage")]
        [Produces(typeof(IEnumerable<TranslationView>))]
        public async Task<IActionResult> GetTranslationForWord(int id, int wordId, Languages language)
        {
            var request = new GetTranslationForWordLanguageRequest(id)
            {
                WordId = wordId,
                Language = language
            };

            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/translations/{translationId}", Name = "GetTranslationById")]
        [Produces(typeof(TranslationView))]
        public async Task<IActionResult> Get(int id, long wordId, int translationId)
        {
            var request = new GetTranslationRequest(id, wordId, translationId);

            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpPost("api/dictionaries/{id}/words/{wordId}/translations", Name = "AddTranslation")]
        [Produces(typeof(TranslationView))]
        [ValidateModel]
        public async Task<IActionResult> Post(int id, int wordId, [FromBody]TranslationView translation)
        {
            var request = new PostTranslationRequest(id)
            {
                WordId = wordId,
                Translation = translation
            };

            await _commandProcessor.SendAsync(request);
            return Created(request.Result.Location, request.Result.Response);
        }

        [HttpPut("api/dictionaries/{id}/words/{wordId}/translations/{translationId}", Name = "UpdateTranslation")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, long wordId, int translationId, [FromBody]TranslationView translation)
        {

            var request = new PutTranslationRequest(id, wordId, translationId)
            {
                Translation = translation
            };

            await _commandProcessor.SendAsync(request);
            return NoContent();
        }

        [HttpDelete("api/dictionaries/{id}/words/{wordId}/translations/{translationId}", Name = "DeleteTranslation")]
        public async Task<IActionResult> Delete(int id, long wordId, int translationId)
        {
            var request = new DeleteTranslationRequest(id, wordId, translationId);

            await _commandProcessor.SendAsync(request);
            return NoContent();
        }
    }
}