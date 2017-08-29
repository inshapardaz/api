using System.Linq;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using paramore.brighter.commandprocessor;

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
        public async Task<IActionResult> Get(int id, int pageNumber = 1, int pageSize = 10)
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

        [HttpGet("api/words/{id}", Name = "GetWordById")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = _userHelper.GetUserId();
            var word = await _queryProcessor.ExecuteAsync(new WordByIdQuery { Id = id, UserId = userId });
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
            var dictionary =
                await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { DictionaryId = id, UserId = userId });

            if (userId == null || dictionary == null)
            {
                return Unauthorized();
            }

            var addWordCommand = new AddWordCommand { Word = word.Map<WordView, Word>() };
            addWordCommand.Word.DictionaryId = id;
            await _commandProcessor.SendAsync(addWordCommand);

            var response = _wordRenderer.Render(addWordCommand.Word);
            return new CreatedResult(response.Links.Single(x => x.Rel == "self").Href, response);
        }

        [HttpPut("/api/words/{id}", Name = "UpdateWord")]
        public async Task<IActionResult> Put(int id, [FromBody] WordView word)
        {
            if (word == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userHelper.GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var response = await _queryProcessor.ExecuteAsync(new WordByIdQuery { Id = id, UserId = userId });

            if (response == null)
            {
                return NotFound();
            }

            await _commandProcessor.SendAsync(new UpdateWordCommand { Word = word.Map<WordView, Word>() });

            return NoContent();
        }

        [HttpDelete("/api/words/{id}", Name = "DeleteWord")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userHelper.GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var word = await _queryProcessor.ExecuteAsync(new WordByIdQuery { Id = id, UserId = userId });

            if (word == null)
            {
                return NotFound();
            }

            await _commandProcessor.SendAsync(new DeleteWordCommand { WordId = word.Id });

            return NoContent();
        }

        #region Unwanted methods

        //[HttpGet("/api/word/exists/{word}")]
        //public IActionResult Exists(string word)
        //{
        //    var result = _queryProcessor.Execute(new WordByTitleQuery { Title = word });

        //    if (result == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok();
        //}

        //[HttpPost("/api/[controller]/{id}/merge", Name = "MergeWords")]
        //public IActionResult Merge(int id, [FromBody]MergeWordViewModel target)
        //{
        //    return new StatusCodeResult(501);
        //}

        #endregion Unwanted methods
    }
}