using System;
using System.Linq;
using System.Threading.Tasks;
using Paramore.Darker;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers
{
    public class WordController : Controller
    {
        private readonly IRenderResponseFromObject<Word, WordView> _wordRenderer;
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderResponseFromObject<PageRendererArgs<Word>, PageView<WordView>> _pageRenderer;

        public WordController(IRenderResponseFromObject<Word, WordView> wordRenderer,
            IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IUserHelper userHelper,
            IRenderResponseFromObject<PageRendererArgs<Word>, PageView<WordView>> pageRenderer)
        {
            _wordRenderer = wordRenderer;
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _pageRenderer = pageRenderer;
        }

        [HttpGet]
        [Route("api/dictionaries/{id}/words", Name = "GetWords")]
        public async Task<IActionResult> GetWords(int id, int pageNumber = 1, int pageSize = 10)
        {
            var userId = _userHelper.GetUserId();
            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { DictionaryId = id });

            if (dictionary == null)
            {
                return NotFound();
            }

            if (!dictionary.IsPublic && dictionary.UserId != userId)
            {
                return Unauthorized();
            }

            var query = new GetWordPageQuery
            {
                DictionaryId = id,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var results = await _queryProcessor.ExecuteAsync(query);

            var pageRenderArgs = new PageRendererArgs<Word>
            {
                RouteName = "GetWords",
                Page = results
            };

            return Ok(_pageRenderer.Render(pageRenderArgs));
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}", Name = "GetWordById")]
        public async Task<IActionResult> GetWord(int id, int wordId)
        {
            var userId = _userHelper.GetUserId();
            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { DictionaryId = id });

            if (dictionary == null)
            {
                return NotFound();
            }

            if (!dictionary.IsPublic && dictionary.UserId != userId)
            {
                return Unauthorized();
            }

            var word = await _queryProcessor.ExecuteAsync(new WordByIdQuery { DictionaryId = id, WordId = wordId, UserId = userId });
            if (word == null)
            {
                return NotFound();
            }

            return Ok(_wordRenderer.Render(word));
        }

        [HttpPost("/api/dictionaries/{id}/words", Name = "CreateWord")]
        public async Task<IActionResult> Post(int id, [FromBody] WordView word)
        {
            if (word == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userHelper.GetUserId();
            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { DictionaryId = id, UserId = userId });

            if (userId == null || dictionary == null)
            {
                return Unauthorized();
            }

            var addWordCommand = new AddWordCommand { DictionaryId = id,  Word = word.Map<WordView, Word>() };
            addWordCommand.Word.DictionaryId = id;
            await _commandProcessor.SendAsync(addWordCommand);

            var response = _wordRenderer.Render(addWordCommand.Word);
            return new CreatedResult(response.Links.Single(x => x.Rel == "self").Href, response);
        }

        [HttpPut("/api/dictionaries/{id}/words/{wordId}", Name = "UpdateWord")]
        public async Task<IActionResult> Put(int id, int wordId, [FromBody] WordView word)
        {
            if (word == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userHelper.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { DictionaryId = id });

            if (dictionary == null)
            {
                return NotFound();
            }

            var response = await _queryProcessor.ExecuteAsync(new WordByIdQuery { WordId = wordId, UserId = userId });

            if (response == null)
            {
                return NotFound();
            }

            await _commandProcessor.SendAsync(new UpdateWordCommand { Word = word.Map<WordView, Word>() });

            return NoContent();
        }

        [HttpDelete("/api/dictionaries/{id}/words/{wordId}", Name = "DeleteWord")]
        public async Task<IActionResult> Delete(int id, int wordId)
        {
            var userId = _userHelper.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { DictionaryId = id });

            if (dictionary == null)
            {
                return NotFound();
            }

            if (!dictionary.IsPublic && dictionary.UserId != userId)
            {
                return Unauthorized();
            }

            var word = await _queryProcessor.ExecuteAsync(new WordByIdQuery { WordId = wordId, UserId = userId });

            if (word == null)
            {
                return NotFound();
            }

            await _commandProcessor.SendAsync(new DeleteWordCommand { WordId = word.Id });

            return NoContent();
        }
    }
}