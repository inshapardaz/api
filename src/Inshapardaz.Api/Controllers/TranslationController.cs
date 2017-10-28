using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using System.Threading.Tasks;
using Inshapardaz.Api.Adapters.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Controllers
{
    public class TranslationController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;

        public TranslationController(IAmACommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/translations", Name = "GetWordTranslationsById")]
        public async Task<IActionResult> GetTranslationForWord(int id, int wordId)
        {
            var request = new GetTranslationForWordRequest
            {
                DictionaryId = id,
                WordId = wordId
            };

            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/translations/languages/{language}", Name = "GetWordTranslationsByWordIdAndLanguage")]
        public async Task<IActionResult> GetTranslationForWord(int id, int wordId, Languages language)
        {
            var request = new GetTranslationForWordLanguageRequest
            {
                DictionaryId = id,
                WordId = wordId,
                Language = language
            };

            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpGet("api/dictionaries/{id}/detail/{id}/translations", Name = "GetWordTranslationsByDetailId")]
        public async Task<IActionResult> GetTranslationForWordDetail(int id, int wordId)
        {
            var request = new GetTranslationForWordDetailRequest
            {
                DictionaryId = id,
                WordDetailId = wordId
            };

            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpGet("api/dictionaries/{id}/detail/{id}/translations/languages/{language}", Name = "GetWordTranslationsByDetailIdAndLanguage")]
        public async Task<IActionResult> GetTranslationForWordDetail(int id, int wordId, Languages language)
        {
            var request = new GetTranslationForWordDetailLanguageRequest
            {
                DictionaryId = id,
                WordDetailId = wordId
            };

            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpGet("api/dictionaries/{id}/translations/{detailId}", Name = "GetTranslationById")]
        public async Task<IActionResult> Get(int id, int translationId)
        {
            var request = new GetTranslationRequest
            {
                DictionaryId = id,
                TranslationId = translationId
            };

            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpPost("api/dictionaries/{id}/details/{detailId}/translations", Name = "AddTranslation")]
        public async Task<IActionResult> Post(int id, int detailId, [FromBody]TranslationView translation)
        {
            var request = new PostTranslationRequest
            {
                DictionaryId = id,
                WordDetailId = detailId,
                Translation = translation
            };

            await _commandProcessor.SendAsync(request);
            return Created(request.Result.Location, request.Result.Response);
        }

        [HttpPut("api/dictionaries/{id}/translations/{translationId}", Name = "UpdateTranslation")]
        public async Task<IActionResult> Put(int id, int translationId, [FromBody]TranslationView translation)
        {

            var request = new PutTranslationRequest
            {
                DictionaryId = id,
                TranslationId = translationId,
                Translation = translation
            };

            await _commandProcessor.SendAsync(request);
            return NoContent();
        }

        [HttpDelete("api/dictionaries/{id}/translations/{translationId}", Name = "DeleteTranslation")]
        public async Task<IActionResult> Delete(int id, int translationId)
        {
            var request = new DeleteTranslationRequest
            {
                DictionaryId = id,
                TranslationId = translationId
            };

            await _commandProcessor.SendAsync(request);
            return NoContent();
        }
    }
}