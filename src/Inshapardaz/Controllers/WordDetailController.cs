using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [Route("/api/word/{id}/Details", Name = "GetWordDetailsById")]
        public async Task<IActionResult> GetForWord(int id)
        {
            var dictionary = await _queryProcessor.ExecuteAsync(new GetDictionaryByWordIdQuery {WordId = id});
            if (dictionary == null || dictionary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var query = new WordDetailsByWordQuery
            {
                WordId = id,
                IncludeDetails = true
            };

            var wordDetailViews = await _queryProcessor.ExecuteAsync(query);
            if (wordDetailViews == null || !wordDetailViews.Any())
            {
                return NotFound();
            }

            return Ok(wordDetailViews.Select(w => _wordDetailRenderer.Render(w)).ToList());
        }

        [HttpGet]
        [Route("/api/Word/Details/{id}", Name = "GetDetailsById")]
        public async Task<IActionResult> Get(int id)
        {
            var dictionary = await _queryProcessor.ExecuteAsync(new GetDictionaryByWordDetailIdQuery { WordDetailId = id});
            if (dictionary == null || dictionary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery {Id = id});

            if (details == null)
            {
                return NotFound();
            }

            return Ok(_wordDetailRenderer.Render(details));
        }

        [HttpPost("/api/word/{id}/Details", Name = "AddWordDetail")]
        public async Task<IActionResult> Post(int id, [FromBody]WordDetailView wordDetail)
        {
            if (wordDetail == null)
            {
                return BadRequest();
            }

            var dictonary = await _queryProcessor.ExecuteAsync(new GetDictionaryByWordIdQuery { WordId = id });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var response = await _queryProcessor.ExecuteAsync(new WordByIdQuery{ Id = id});

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
            return new CreatedResult(responseView.Links.Single(x => x.Rel == "self").Href, responseView);
        }

        [HttpPut("/api/Details/{wordDetailId}", Name = "UpdateWordDetail")]
        public async Task<IActionResult> Put(int wordDetailId, [FromBody]WordDetailView wordDetail)
        {
            if (wordDetail == null)
            {
                return BadRequest();
            }

            var dictonary = await _queryProcessor.ExecuteAsync(new GetDictionaryByWordDetailIdQuery { WordDetailId = wordDetailId });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { Id = wordDetailId });

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

            return Ok();
        }

        [HttpDelete("/api/details/{wordDetailId}", Name = "DeleteWordDetail")]
        public async Task<IActionResult> Delete(int id)
        {
            var dictonary = await _queryProcessor.ExecuteAsync(new GetDictionaryByWordDetailIdQuery { WordDetailId = id });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery {Id = id});

            if (details == null)
            {
                return NotFound();
            }

            _commandProcessor.Send(new DeleteWordDetailCommand { WordDetailId =  id});

            return NoContent();
        }
    }
}
