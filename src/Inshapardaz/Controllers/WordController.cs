using System.Linq;
using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Helpers;
using Inshapardaz.Model;
using Inshapardaz.Renderers;
using Microsoft.AspNetCore.Mvc;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Controllers
{
    [Route("api/[controller]")]
    public class WordController : Controller
    {
        private readonly IRenderResponseFromObject<Word, WordView> _wordRenderer;
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        public WordController(IRenderResponseFromObject<Word, WordView> wordRenderer,
            IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor)
        {
            _wordRenderer = wordRenderer;
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        [HttpGet("{id}", Name = "GetWordById")]
        public IActionResult Get(int id)
        {
            var word = _queryProcessor.Execute(new WordByIdQuery { Id = id });
            if (word == null)
            {
                return NotFound();
            }

            return new ObjectResult(_wordRenderer.Render(word));
        }

        [HttpGet("/api/word/exists/{word}")]
        public IActionResult Exists(string word)
        {
            var result = _queryProcessor.Execute(new WordByTitleQuery { Title = word });

            if (result == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost("/api/[controller]/{id}/merge", Name="MergeWords")]
        public IActionResult Merge(int id, [FromBody]MergeWordViewModel target)
        {
            return new StatusCodeResult(501);
        }

        [HttpPost("/api/dictionary/{id}/word", Name = "CreateWord")]
        public IActionResult Post(int id, [FromBody]WordView word)
        {
            if (word == null)
            {
                return BadRequest();
            }

            var addWordCommand = new AddWordCommand { Word = word.Map<WordView, Word>()};
            addWordCommand.Word.DictionaryId = id;
            _commandProcessor.Send(addWordCommand);

            var response = _wordRenderer.Render(addWordCommand.Word);
            return new CreatedResult(response.Links.Single(x => x.Rel == "self").Href, response);
        }

        [HttpPut("/api/dictionary/{did}/Word/{id}", Name="UpdateWord")]
        public IActionResult Put(int did, int id, [FromBody]WordView word)
        {
            if (word == null || id != word.Id)
            {
                return BadRequest();
            }

            var response = _queryProcessor.Execute(new WordByIdQuery { Id = id });

            if (response == null)
            {
                return NotFound();
            }

            _commandProcessor.Send(new UpdateWordCommand { Word = word.Map<WordView, Word>() });

            return Ok();
        }

        [HttpDelete("/api/word/{id}", Name="DeleteWord")]
        public IActionResult Delete(int id)
        {
            var word = _queryProcessor.Execute(new WordByIdQuery {Id =  id});

            if (word == null)
            {
                return NotFound();
            }

            _commandProcessor.Send(new DeleteWordCommand { Word = word });

            return Ok();
        }
    }
}
