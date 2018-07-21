using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Ports.Dictionary;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers.Dictionary
{
    public class TranslationController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderTranslation _translationRenderer;

        public TranslationController(IAmACommandProcessor commandProcessor, IRenderTranslation translationRenderer)
        {
            _commandProcessor = commandProcessor;
            _translationRenderer = translationRenderer;
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/translations", Name = "GetWordTranslationsById")]
        [Produces(typeof(IEnumerable<TranslationView>))]
        public async Task<IActionResult> GetTranslationForWord(int id, int wordId)
        {
            var request = new GetTranslationForWordRequest(id, wordId);

            await _commandProcessor.SendAsync(request);
            return Ok(request.Result.Select(t => _translationRenderer.Render(t, id)));
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/translations/languages/{language}", Name = "GetWordTranslationsByWordIdAndLanguage")]
        [Produces(typeof(IEnumerable<TranslationView>))]
        public async Task<IActionResult> GetTranslationForWord(int id, int wordId, Languages language)
        {
            var request = new GetTranslationForWordLanguageRequest(id, wordId, language);

            await _commandProcessor.SendAsync(request);
            return Ok(request.Result.Select(t => _translationRenderer.Render(t, id)));
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/translations/{translationId}", Name = "GetTranslationById")]
        [Produces(typeof(TranslationView))]
        public async Task<IActionResult> Get(int id, long wordId, int translationId)
        {
            var request = new GetTranslationRequest(id, wordId, translationId);

            await _commandProcessor.SendAsync(request);
            return Ok(_translationRenderer.Render(request.Result, id));
        }

        [HttpPost("api/dictionaries/{id}/words/{wordId}/translations", Name = "AddTranslation")]
        [Produces(typeof(TranslationView))]
        [ValidateModel]
        public async Task<IActionResult> Post(int id, int wordId, [FromBody]TranslationView translation)
        {
            var request = new AddTranslationRequest(id, wordId, translation.Map<TranslationView, Translation>());
            await _commandProcessor.SendAsync(request);
            var response = _translationRenderer.Render(request.Result, id);
            return Created(response.Links.Self(), response);
        }

        [HttpPut("api/dictionaries/{id}/words/{wordId}/translations/{translationId}", Name = "UpdateTranslation")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, long wordId, int translationId, [FromBody]TranslationView translation)
        {

            var request = new UpdateTranslationRequest(id, wordId, translation.Map<TranslationView, Translation>());
            await _commandProcessor.SendAsync(request);
            if (request.Result.HasAddedNew)
            {
                var response = _translationRenderer.Render(request.Result.Translation, id);
                return Created(response.Links.Self(), response);
            }

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