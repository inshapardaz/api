using System.Collections.Generic;
using System.Linq;
using Darker;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using paramore.brighter.commandprocessor;
using System.Threading.Tasks;

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

        [Route("api/words/{id}/translations", Name = "GetWordTranslationsById")]
        public async Task<IActionResult> GetTranslationForWord(int id)
        {
            var user = _userHelper.GetUserId();
            if (!string.IsNullOrWhiteSpace(user))
            {
                var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery { WordId = id });
                if (dictionary != null && dictionary.UserId != user)
                {
                    return Unauthorized();
                }
            }

            var translations = await _queryProcessor.ExecuteAsync(new TranslationsByWordIdQuery { WordId = id });
            return Ok(translations.Select(t => _translationRenderer.Render(t)).ToList());
        }

        [Route("api/words/{id}/translations/{language}")]
        public async Task<IActionResult> GetTranslationForWord(int id, Languages language)
        {
            var user = _userHelper.GetUserId();
            if (!string.IsNullOrWhiteSpace(user))
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
            if (translation == null)
            {
                return BadRequest();
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
            if (translation == null)
            {
                return BadRequest();
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