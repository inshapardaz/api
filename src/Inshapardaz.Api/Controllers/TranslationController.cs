using System;
using System.Linq;
using Paramore.Darker;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Controllers
{
    public class TranslationController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderResponseFromObject<Translation, TranslationView> _translationRenderer;

        public TranslationController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IUserHelper userHelper,
            IRenderResponseFromObject<Translation, TranslationView> renderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _translationRenderer = renderer;
            _userHelper = userHelper;
        }

        [HttpGet("api/words/{id}/translations", Name = "GetWordTranslationsById")]
        public async Task<IActionResult> GetTranslationForWord(int id)
        {
            var user = _userHelper.GetUserId();
            if (user != Guid.Empty)
            {
                var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery { WordId = id });
                if (dictionary != null &&  dictionary.UserId != user)
                {
                    return Unauthorized();
                }
            }

            var translations = await _queryProcessor.ExecuteAsync(new TranslationsByWordIdQuery { WordId = id });
            return Ok(translations.Select(t => _translationRenderer.Render(t)).ToList());
        }

        [HttpGet("api/words/{id}/translations/languages/{language}", Name = "GetWordTranslationsByWordIdAndLanguage")]
        public async Task<IActionResult> GetTranslationForWord(int id, Languages language)
        {
            var user = _userHelper.GetUserId();
            if (user != Guid.Empty)
            {
                var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery { WordId = id });
                if (dictionary != null && dictionary.UserId != user)
                {
                    return Unauthorized();
                }
            }

            var translations = await _queryProcessor.ExecuteAsync(new TranslationsByLanguageQuery { WordId = id, Language = language });
            return Ok(translations.Select(t => _translationRenderer.Render(t)).ToList());
        }

        [HttpGet("api/detail/{id}/translations", Name = "GetWordTranslationsByDetailId")]
        public async Task<IActionResult> GetTranslationForWordDetail(int id)
        {
            var user = _userHelper.GetUserId();
            if (user != Guid.Empty)
            {
                var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByWordDetailIdQuery { WordDetailId = id });
                if (dictionary != null && dictionary.UserId != user)
                {
                    return Unauthorized();
                }
            }

            var translations = await _queryProcessor.ExecuteAsync(new TranslationsByWordDetailIdQuery { WordDetailId = id });
            return Ok(translations.Select(t => _translationRenderer.Render(t)).ToList());
        }

        [HttpGet("api/detail/{id}/translations/languages/{language}", Name = "GetWordTranslationsByDetailIdAndLanguage")]
        public async Task<IActionResult> GetTranslationForWordDetail(int id, Languages language)
        {
            var user = _userHelper.GetUserId();
            if (user != Guid.Empty)
            {
                var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByWordDetailIdQuery { WordDetailId = id });
                if (dictionary != null && dictionary.UserId != user)
                {
                    return Unauthorized();
                }
            }

            var translations = await _queryProcessor.ExecuteAsync(new TranslationsByWordDetailAndLanguageQuery { WordDetailId = id, Language = language });
            return Ok(translations.Select(t => _translationRenderer.Render(t)).ToList());
        }

        [HttpGet("api/translations/{id}", Name = "GetTranslationById")]
        public IActionResult Get(int id)
        {
            var response = _queryProcessor.Execute(new TranslationByIdQuery { Id = id });

            if (response == null)
            {
                return NotFound();
            }

            return new ObjectResult(_translationRenderer.Render(response));
        }

        [HttpPost("api/details/{id}/translations", Name = "AddTranslation")]
        public async Task<IActionResult> Post(int id, [FromBody]TranslationView translation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dictonary = await _queryProcessor.ExecuteAsync(new DictionaryByWordDetailIdQuery { WordDetailId = id });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var dwtails = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { Id = id });

            if (dwtails == null)
            {
                return BadRequest();
            }

            var command = new AddWordTranslationCommand
            {
                WordDetailId = id,
                Translation = translation.Map<TranslationView, Translation>()
            };
            await _commandProcessor.SendAsync(command);

            var response = _translationRenderer.Render(command.Translation);

            return Created(response.Links.Single(x => x.Rel == "self").Href, response);
        }

        [HttpPut("api/translations/{id}", Name = "UpdateTranslation")]
        public async Task<IActionResult> Put(int id, [FromBody]TranslationView translation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dictonary = await _queryProcessor.ExecuteAsync(new DictionaryByTranslationIdQuery { TranslationId = id });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var response = await _queryProcessor.ExecuteAsync(new TranslationByIdQuery { Id = id });

            if (response == null)
            {
                return BadRequest();
            }

            await _commandProcessor.SendAsync(new UpdateWordTranslationCommand { Translation = translation.Map<TranslationView, Translation>() });

            return NoContent();
        }

        [HttpDelete("api/translations/{id}", Name = "DeleteTranslation")]
        public async Task<IActionResult> Delete(int id)
        {
            var dictonary = await _queryProcessor.ExecuteAsync(new DictionaryByTranslationIdQuery { TranslationId = id });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var response = await _queryProcessor.ExecuteAsync(new TranslationByIdQuery { Id = id });

            if (response == null)
            {
                return NotFound();
            }

            await _commandProcessor.SendAsync(new DeleteWordTranslationCommand { TranslationId = id });

            return NoContent();
        }
    }
}