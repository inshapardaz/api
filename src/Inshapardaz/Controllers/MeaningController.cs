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
    public class MeaningController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        private readonly IRenderResponseFromObject<Meaning, MeaningView> _meaningRenderer;

        public MeaningController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderResponseFromObject<Meaning, MeaningView> meaningRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _meaningRenderer = meaningRenderer;
        }

        [HttpGet("{id}", Name = "GetMeaningById")]
        public IActionResult Get(int id)
        {
            var meaning = _queryProcessor.Execute(new WordMeaningByIdQuery {Id = id});

            if (meaning == null)
            {
                return NotFound();
            }

            return new ObjectResult(_meaningRenderer.Render(meaning));
        }

        [Route("api/word/{id}/Meaning", Name = "GetWordMeaningById")]
        public IEnumerable<MeaningView> GetMeaningForWord(int id)
        {
            return _queryProcessor.Execute(new WordMeaningByWordQuery { WordId = id } )
                                  .Select(x => _meaningRenderer.Render(x));
        }

        [Route("api/word/{id}/Meaning/{context}", Name = "GetWordMeaningByContext")]
        public IEnumerable<MeaningView> GetMeaningForWord(int id, string context)
        {
            var finalContext = string.Empty;
            if (context != "default")
            {
                finalContext = context;
            }

            return _queryProcessor.Execute(new WordMeaningByWordQuery { WordId = id, Context = finalContext })
                                   .Select(x => _meaningRenderer.Render(x));
        }

        [HttpPost("api/word/{id}/detail/{detailId}/meaning", Name = "AddMeaning")]
        public IActionResult Post(int id, int detailId, [FromBody]MeaningView meaning)
        {
            if (meaning == null)
            {
                return BadRequest();
            }

            _commandProcessor.Send(new AddWordMeaningCommand { Meaning = meaning.Map<MeaningView, Meaning>() });
            return Created("http://tempuri", 0);
        }

        [HttpPut("api/word/meaning/{id}", Name = "UpdateMeaning")]
        public IActionResult Put(int id, [FromBody]MeaningView meaning)
        {
            if (meaning == null)
            {
                return BadRequest();
            }

            var response = _queryProcessor.Execute(new WordMeaningByIdQuery { Id = id });

            if (response == null || response.Id != meaning.Id)
            {
                return NotFound();
            }

            _commandProcessor.Send(new UpdateWordMeaningCommand { Meaning = meaning.Map<MeaningView, Meaning>() });

            return Ok();
        }

        [HttpDelete("api/word/meaning/{id}", Name = "DeleteMeaning")]
        public IActionResult Delete(int id)
        {
            var response = _queryProcessor.Execute(new WordMeaningByIdQuery { Id = id });

            if (response == null || response.Id != id)
            {
                return NotFound();
            }

            _commandProcessor.Send(new DeleteWordMeaningCommand { Meaning = response });

            return Ok();
        }
    }
}
