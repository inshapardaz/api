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

        [HttpGet]
        [Route("/api/word/{id}/Details", Name = "GetWordDetailsById")]
        public IEnumerable<WordDetailView> GetForWord(int id)
        {
            var query = new WordDetailsByWordQuery
            {
                WordId = id,
                IncludeDetails = true
            };

            return _queryProcessor.Execute(query)
                                  .Select(w => _wordDetailRenderer.Render(w));
        }

        // POST api/values
        [HttpPost("/api/word/{id}/Details", Name = "AddWordDetail")]
        public IActionResult Post(int id, [FromBody]WordDetailView wordDetail)
        {
            if (wordDetail == null)
            {
                return BadRequest();
            }

            var response = _queryProcessor.Execute(new WordByIdQuery{ Id = id});

            if (response == null)
            {
                return BadRequest();
            }


            _commandProcessor.Send(new AddWordDetailCommand
            {
                WordId = id,
                WordDetail = wordDetail.Map<WordDetailView, WordDetail>()
            });

            return Created("http://tempuri", 0);
        }

        // PUT api/values/5
        [HttpPut("/api/word/{id}/Details/{wordDetailId}", Name = "UpdateWordDetail")]
        public IActionResult Put(int id, int wordDetailId, [FromBody]WordDetailView wordDetail)
        {
            if (wordDetail == null)
            {
                return BadRequest();
            }

            var details = _queryProcessor.Execute(new WordDetailByIdQuery { Id = wordDetailId });

            if (details == null || details.Id != wordDetail.Id)
            {
                return BadRequest();
            }

            _commandProcessor.Send(new UpdateWordDetailCommand
            {
                WordId = id,
                WordDetail = wordDetail.Map<WordDetailView, WordDetail>()
            });

            return Ok();
        }

        // DELETE api/values/5
        [HttpDelete("/api/word/{id}/Details/{wordDetailId}", Name = "DeleteWordDetail")]
        public IActionResult Delete(int id, int wordDetailId)
        {
            var details = _queryProcessor.Execute(new WordDetailByIdQuery {Id = wordDetailId});

            if (details == null || details.Id != id)
            {
                return NotFound();
            }

            _commandProcessor.Send(new DeleteWordDetailCommand { WordDetailId =  id});

            return Ok();
        }
    }
}
