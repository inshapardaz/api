using System;
using System.Collections.Generic;
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
    public class MeaningController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderResponseFromObject<Meaning, MeaningView> _meaningRenderer;

        public MeaningController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IUserHelper userHelper,
            IRenderResponseFromObject<Meaning, MeaningView> meaningRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _meaningRenderer = meaningRenderer;
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/meanings", Name = "GetWordMeaningByWordId")]
        public async Task<IActionResult> GetMeaningForWord(int id, int wordId)
        {
            IEnumerable<Meaning> meanings = await _queryProcessor.ExecuteAsync(new WordMeaningByWordQuery { WordId = wordId });
            return Ok(meanings.Select(x => _meaningRenderer.Render(x)).ToList());
        }

        [HttpGet("api/dictionaries/{id}/details/{detailId}/meanings", Name = "GetWordMeaningByWordDetailId")]
        public async Task<IActionResult> GetMeaningForWordDetail(int id, int detailId)
        {
            IEnumerable<Meaning> meanings = await _queryProcessor.ExecuteAsync(new WordMeaningByWordDetailQuery { WordDetailId = detailId });
            return Ok(meanings.Select(x => _meaningRenderer.Render(x)).ToList());
        }

        [HttpGet("api/dictionaries/{id}/meanings/{meaningId}", Name = "GetMeaningById")]
        public async Task<IActionResult> Get(int id, int meaningId)
        {
            var user = _userHelper.GetUserId();
            if (user != Guid.Empty)
            {
                var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByMeaningIdQuery { MeaningId = meaningId });
                if (dictionary != null && dictionary.UserId != user)
                {
                    return Unauthorized();
                }
            }

            var meaning = await _queryProcessor.ExecuteAsync(new WordMeaningByIdQuery { Id = meaningId });

            if (meaning == null)
            {
                return NotFound();
            }

            return Ok(_meaningRenderer.Render(meaning));
        }

        [HttpGet("api/dictionaries/{id}/words/{wordId}/meanings/contexts/{context}", Name = "GetWordMeaningByContext")]
        public IEnumerable<MeaningView> GetMeaningForContext(int id, int wordId, string context)
        {
            var finalContext = string.Empty;
            if (context != "default")
            {
                finalContext = context;
            }

            return _queryProcessor.Execute(new WordMeaningByWordQuery { WordId = wordId, Context = finalContext })
                                   .Select(x => _meaningRenderer.Render(x));
        }

        [HttpPost("api/dictionaries/{id}/details/{detailId}/meanings", Name = "AddMeaning")]
        public async Task<IActionResult> Post(int id, int detailId, [FromBody]MeaningView meaning)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByWordDetailIdQuery { WordDetailId = detailId });
            if (dictionary == null || dictionary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var detail = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { WordDetailId = detailId });

            if (detail == null)
            {
                return BadRequest();
            }

            var command = new AddWordMeaningCommand { WordDetailId = detail.Id, Meaning = meaning.Map<MeaningView, Meaning>()};
            await _commandProcessor.SendAsync(command);
            var response = _meaningRenderer.Render(command.Meaning);
            return Created(response.Links.Single(x => x.Rel == "self").Href, response);
        }

        [HttpPut("api/dictionaries/{id}/meanings/{meaningId}", Name = "UpdateMeaning")]
        public async Task<IActionResult> Put(int id, int meaningId, [FromBody]MeaningView meaning)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByMeaningIdQuery { MeaningId = meaningId });
            if (dictionary == null || dictionary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var response = await _queryProcessor.ExecuteAsync(new WordMeaningByIdQuery { Id = meaningId });

            if (response == null || response.Id != meaning.Id)
            {
                return BadRequest();
            }

            await _commandProcessor.SendAsync(new UpdateWordMeaningCommand { Meaning = meaning.Map<MeaningView, Meaning>() });

            return NoContent();
        }

        [HttpDelete("api/dictionaries/{id}/meanings/{meaningId}", Name = "DeleteMeaning")]
        public async Task<IActionResult> Delete(int id, int meaningId)
        {
            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByMeaningIdQuery { MeaningId = meaningId });
            if (dictionary == null || dictionary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var response = await _queryProcessor.ExecuteAsync(new WordMeaningByIdQuery { Id = meaningId });

            if (response == null)
            {
                return NotFound();
            }

            await _commandProcessor.SendAsync(new DeleteWordMeaningCommand { MeaningId = response.Id });

            return NoContent();
        }
    }
}