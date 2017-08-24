using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Api.Controllers
{
    public class WordDetailController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;

        private readonly IRenderResponseFromObject<WordDetail, WordDetailView> _wordDetailRenderer;

        public WordDetailController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IUserHelper userHelper,
            IRenderResponseFromObject<WordDetail, WordDetailView> wordDetailRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _wordDetailRenderer = wordDetailRenderer;
        }

        [HttpGet]
        [Route("/api/words/{id}/details", Name = "GetWordDetailsById")]
        public async Task<IActionResult> GetForWord(int id)
        {
            var query = new WordDetailsByWordQuery
            {
                WordId = id
            };

            var wordDetailViews = await _queryProcessor.ExecuteAsync(query);
            return Ok(wordDetailViews.Select(w => _wordDetailRenderer.Render(w)).ToList());
        }

        [HttpGet]
        [Route("/api/details/{id}", Name = "GetDetailsById")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = _userHelper.GetUserId();

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByWordDetailIdQuery { WordDetailId = id });

                if (dictionary == null || dictionary.UserId != userId)
                {
                    return Unauthorized();
                }
            }

            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { Id = id });

            if (details == null)
            {
                return NotFound();
            }

            return Ok(_wordDetailRenderer.Render(details));
        }

        [HttpPost("/api/words/{id}/details", Name = "AddWordDetail")]
        public async Task<IActionResult> Post(int id, [FromBody]WordDetailView wordDetail)
        {
            if (wordDetail == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dictonary = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery { WordId = id });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var response = await _queryProcessor.ExecuteAsync(new WordByIdQuery { Id = id });

            if (response == null)
            {
                return BadRequest();
            }

            var addWordDetailCommand = new AddWordDetailCommand
            {
                WordId = id,
                WordDetail = wordDetail.Map<WordDetailView, WordDetail>()
            };

            await _commandProcessor.SendAsync(addWordDetailCommand);

            var responseView = _wordDetailRenderer.Render(addWordDetailCommand.WordDetail);
            return Created(responseView.Links.Single(x => x.Rel == "self").Href, responseView);
        }

        [HttpPut("/api/details/{id}", Name = "UpdateWordDetail")]
        public async Task<IActionResult> Put(int id, [FromBody]WordDetailView wordDetail)
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

            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { Id = id });

            if (details == null || details.Id != wordDetail.Id)
            {
                return BadRequest();
            }

            var updateWordDetailCommand = new UpdateWordDetailCommand
            {
                WordId = details.WordInstanceId,
                WordDetail = wordDetail.Map<WordDetailView, WordDetail>()
            };

            await _commandProcessor.SendAsync(updateWordDetailCommand);

            return NoContent();
        }

        [HttpDelete("/api/details/{id}", Name = "DeleteWordDetail")]
        public async Task<IActionResult> Delete(int id)
        {
            var dictonary = await _queryProcessor.ExecuteAsync(new DictionaryByWordDetailIdQuery { WordDetailId = id });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { Id = id });

            if (details == null)
            {
                return NotFound();
            }

            await _commandProcessor.SendAsync(new DeleteWordDetailCommand { WordDetailId = id });

            return NoContent();
        }
    }
}