using System;
using System.Collections.Generic;
using System.Linq;
using Darker;
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

        [HttpGet("api/words/{id}/meanings", Name = "GetWordMeaningByWordId")]
        public async Task<IActionResult> GetMeaningForWord(int id)
        {
            IEnumerable<Meaning> meanings = await _queryProcessor.ExecuteAsync(new WordMeaningByWordQuery { WordId = id });
            return Ok(meanings.Select(x => _meaningRenderer.Render(x)).ToList());
        }

        [HttpGet("api/details/{id}/meanings", Name = "GetWordMeaningByWordDetailId")]
        public async Task<IActionResult> GetMeaningForWordDetail(int id)
        {
            IEnumerable<Meaning> meanings = await _queryProcessor.ExecuteAsync(new WordMeaningByWordDetailQuery { WordDetailId = id });
            return Ok(meanings.Select(x => _meaningRenderer.Render(x)).ToList());
        }

        [HttpGet("api/meanings/{id}", Name = "GetMeaningById")]
        public async Task<IActionResult> Get(int id)
        {
            var user = _userHelper.GetUserId();
            if (user != Guid.Empty)
            {
                var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByMeaningIdQuery { MeaningId = id });
                if (dictionary != null && dictionary.UserId != user)
                {
                    return Unauthorized();
                }
            }

            var meaning = await _queryProcessor.ExecuteAsync(new WordMeaningByIdQuery { Id = id });

            if (meaning == null)
            {
                return NotFound();
            }

            return Ok(_meaningRenderer.Render(meaning));
        }

        [HttpGet("api/words/{id}/meanings/contexts/{context}", Name = "GetWordMeaningByContext")]
        public IEnumerable<MeaningView> GetMeaningForContext(int id, string context)
        {
            var finalContext = string.Empty;
            if (context != "default")
            {
                finalContext = context;
            }

            return _queryProcessor.Execute(new WordMeaningByWordQuery { WordId = id, Context = finalContext })
                                   .Select(x => _meaningRenderer.Render(x));
        }

        [HttpPost("api/details/{id}/meanings", Name = "AddMeaning")]
        public async Task<IActionResult> Post(int id, [FromBody]MeaningView meaning)
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

            var detail = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { Id = id });

            if (detail == null)
            {
                return BadRequest();
            }

            var command = new AddWordMeaningCommand { WordDetailId = detail.Id, Meaning = meaning.Map<MeaningView, Meaning>()};
            await _commandProcessor.SendAsync(command);
            var response = _meaningRenderer.Render(command.Meaning);
            return Created(response.Links.Single(x => x.Rel == "self").Href, response);
        }

        [HttpPut("api/meanings/{id}", Name = "UpdateMeaning")]
        public async Task<IActionResult> Put(int id, [FromBody]MeaningView meaning)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dictonary = await _queryProcessor.ExecuteAsync(new DictionaryByMeaningIdQuery { MeaningId = id });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var response = await _queryProcessor.ExecuteAsync(new WordMeaningByIdQuery { Id = id });

            if (response == null || response.Id != meaning.Id)
            {
                return BadRequest();
            }

            await _commandProcessor.SendAsync(new UpdateWordMeaningCommand { Meaning = meaning.Map<MeaningView, Meaning>() });

            return NoContent();
        }

        [HttpDelete("api/meanings/{id}", Name = "DeleteMeaning")]
        public async Task<IActionResult> Delete(int id)
        {
            var dictonary = await _queryProcessor.ExecuteAsync(new DictionaryByMeaningIdQuery { MeaningId = id });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var response = await _queryProcessor.ExecuteAsync(new WordMeaningByIdQuery { Id = id });

            if (response == null)
            {
                return NotFound();
            }

            await _commandProcessor.SendAsync(new DeleteWordMeaningCommand { MeaningId = response.Id });

            return NoContent();
        }
    }
}