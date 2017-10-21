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

        [HttpGet("/api/dictionaries/{id}/words/{wordId}/details", Name = "GetWordDetailsById")]
        public async Task<IActionResult> GetDetailForWord(int id, int wordId)
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

            var query = new WordDetailsByWordQuery
            {
                DictionaryId = id,
                WordId = wordId
            };

            var wordDetailViews = await _queryProcessor.ExecuteAsync(query);
            return Ok(wordDetailViews.Select(w => _wordDetailRenderer.Render(w)).ToList());
        }

        [HttpGet("/api/dictionaries/{id}/details/{detailId}", Name = "GetDetailsById")]
        public async Task<IActionResult> GetWordDetailById(int id, int detailId)
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

            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { DictionaryId = id, WordDetailId = detailId });

            if (details == null)
            {
                return NotFound();
            }

            return Ok(_wordDetailRenderer.Render(details));
        }

        [HttpPost("/api/dictionaries/{id}/words/{wordId}/details", Name = "AddWordDetail")]
        public async Task<IActionResult> Post(int id, int wordId, [FromBody]WordDetailView wordDetail)
        {
            if (wordDetail == null || !ModelState.IsValid)
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

            if (dictionary.UserId != userId)
            {
                return Unauthorized();
            }

            var response = await _queryProcessor.ExecuteAsync(new WordByIdQuery { DictionaryId = id, WordId = wordId });

            if (response == null)
            {
                return BadRequest();
            }

            var addWordDetailCommand = new AddWordDetailCommand
            {
                DictionaryId = id,
                WordId = wordId,
                WordDetail = wordDetail.Map<WordDetailView, WordDetail>()
            };

            await _commandProcessor.SendAsync(addWordDetailCommand);

            var responseView = _wordDetailRenderer.Render(addWordDetailCommand.WordDetail);
            return Created(responseView.Links.Single(x => x.Rel == "self").Href, responseView);
        }
        
        [HttpPut("/api/dictionaries/{id}/details/{detailId}", Name = "UpdateWordDetail")]
        public async Task<IActionResult> Put(int id, int detailId, [FromBody]WordDetailView wordDetail)
        {
            if (!ModelState.IsValid)
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

            if (dictionary.UserId != userId)
            {
                return Unauthorized();
            }

            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery  { DictionaryId = id, WordDetailId = detailId });

            if (details == null || details.Id != wordDetail.Id)
            {
                return BadRequest();
            }

            var updateWordDetailCommand = new UpdateWordDetailCommand
            {
                DictionaryId = id,
                WordId = details.WordInstanceId,
                WordDetail = wordDetail.Map<WordDetailView, WordDetail>()
            };

            await _commandProcessor.SendAsync(updateWordDetailCommand);

            return NoContent();
        }

        [HttpDelete("/api/dictionaries/{id}/details/{detailId}", Name = "DeleteWordDetail")]
        public async Task<IActionResult> Delete(int id, int detailId)
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
            
            if (dictionary.UserId != userId)
            {
                return Unauthorized();
            }

            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { DictionaryId = id, WordDetailId = detailId });

            if (details == null)
            {
                return NotFound();
            }

            await _commandProcessor.SendAsync(new DeleteWordDetailCommand { DictionaryId = id, WordDetailId = detailId });

            return NoContent();
        }
    }
}