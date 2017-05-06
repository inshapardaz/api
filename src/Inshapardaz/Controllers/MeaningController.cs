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
using System.Threading.Tasks;

namespace Inshapardaz.Api.Controllers
{
    [Route("api/[controller]")]
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

        [Route("api/word/{id}/Meaning", Name = "GetWordMeaningById")]
        public async Task<IActionResult> GetMeaningForWord(int id)
        {
            var user = _userHelper.GetUserId();
            if (!string.IsNullOrWhiteSpace(user))
            {
                var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByWordDetailIdQuery { WordDetailId = id });
                if (dictionary != null && dictionary.UserId != user)
                {
                    return Unauthorized();
                }
            }

            IEnumerable<Meaning> meanings = await _queryProcessor.ExecuteAsync(new WordMeaningByWordQuery { WordId = id });
            return Ok(meanings.Select(x => _meaningRenderer.Render(x)).ToList());
        }

        [HttpGet("{id}", Name = "GetMeaningById")]
        public async Task<IActionResult> Get(int id)
        {
            var user = _userHelper.GetUserId();
            if (!string.IsNullOrWhiteSpace(user))
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

        [HttpPost("api/details/{id}/meaning", Name = "AddMeaning")]
        public async Task<IActionResult> Post(int id, [FromBody]MeaningView meaning)
        {
            if (meaning == null)
            {
                return BadRequest();
            }

            var dictonary = await _queryProcessor.ExecuteAsync(new DictionaryByWordDetailIdQuery { WordDetailId = id });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var dwtails = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { Id = id });

            if (dwtails == null)
            {
                return BadRequest();
            }

            var command = new AddWordMeaningCommand { Meaning = meaning.Map<MeaningView, Meaning>() };
            await _commandProcessor.SendAsync(command);
            var response = _meaningRenderer.Render(command.Meaning);
            return Created(response.Links.Single(x => x.Rel == "self").Href, response);
        }

        [HttpPut("api/meaning/{id}", Name = "UpdateMeaning")]
        public async Task<IActionResult> Put(int id, [FromBody]MeaningView meaning)
        {
            if (meaning == null)
            {
                return BadRequest();
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

        [HttpDelete("api/meaning/{id}", Name = "DeleteMeaning")]
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

            await _commandProcessor.SendAsync(new DeleteWordMeaningCommand { Meaning = response });

            return NoContent();
        }
    }
}