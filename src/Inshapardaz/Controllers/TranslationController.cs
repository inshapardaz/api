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

namespace Inshapardaz.Api.Controllers
{

    [Route("api/[controller]")]
    public class TranslationController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderResponseFromObject<Translation, TranslationView> _renderer;

        public TranslationController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderResponseFromObject<Translation, TranslationView> renderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _renderer = renderer;
        }

        [HttpGet("{id}", Name = "GetTranslationById")]
        public IActionResult Get(int id)
        {
            var response = _queryProcessor.Execute(new TranslationByIdQuery { Id = id });

            if (response == null)
            {
                return NotFound();
            }

            return new ObjectResult(_renderer.Render(response));
        }

        [Route("/api/word/{id}/Translations", Name = "GetWordTranslationsById")]
        public IEnumerable<TranslationView> GetTranslationForWord(int id)
        {
            return _queryProcessor.Execute(new TranslationsByWordIdQuery { WordId = id })
                                  .Select(t => _renderer.Render(t));
        }

        [Route("/api/word/{id}/Translation/{language}")]
        public IEnumerable<TranslationView> GetTranslationForWord(int id, Languages language)
        {
            return _queryProcessor.Execute(new TranslationsByLanguageQuery { WordId = id, Language = language})
                                  .Select(t => _renderer.Render(t));
        }

       [HttpPost("/api/word/{id}/Detail/{detailId}/Translations", Name = "AddTranslation")]
        public IActionResult Post(int id, int detailId, [FromBody]TranslationView translation)
        {
            if (translation == null)
            {
                return BadRequest();
            }

            _commandProcessor.Send(new AddWordTranslationCommand
            {
                WordId = id,
                WordDetailId = detailId,
                Translation = translation.Map<TranslationView, Translation>()
            });

            return Created("http://tempuri", 0);
        }

        [HttpPut("/api/word/{id}/Detail/{detailId}/Translations/{translationId}", Name = "UpdateTranslation")]
        public IActionResult Put(int id, int detailId, int translationId, [FromBody]TranslationView translation)
        {
            if (translation == null)
            {
                return BadRequest();
            }

            var response = _queryProcessor.Execute(new TranslationByIdQuery { Id = translationId });

            if (response == null)
            {
                return NotFound();
            }

            _commandProcessor.Send(new UpdateWordTranslationCommand { Translation = translation.Map<TranslationView, Translation>() });
            
            return Ok();
        }

        [HttpDelete("/api/word/{id}/Detail/{detailId}/Translations/{translationId}", Name = "DeleteTranslation")]
        public IActionResult Delete(int id, int detailId, int translationId)
        {
            var response = _queryProcessor.Execute(new TranslationByIdQuery { Id = id });

            if (response == null )
            {
                return NotFound();
            }

            _commandProcessor.Send(new DeleteWordTranslationCommand { TranslationId = translationId });

            return Ok();
        }
    }
}
