using System.Threading.Tasks;
using Inshapardaz.Api.Adapters.Dictionary;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Ports.Dictionary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers.Dictionary
{
    public class DictionariesController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;

        public DictionariesController(IAmACommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        [HttpGet("/api/dictionaries", Name = "GetDictionaries")]
        [Produces(typeof(DictionariesView))]
        public async Task<IActionResult> GetDictionaries()
        {
            var request = new GetDictionariesRequest();
            await _commandProcessor.SendAsync(request);
            
            return Ok(request.Result);
        }

        [HttpGet("/api/dictionaries/{id}", Name = "GetDictionaryById")]
        [Produces(typeof(DictionaryView))]
        public async Task<IActionResult> GetDictionaryById(int id)
        {
            var request = new GetDictionaryByIdRequest(id);
            await _commandProcessor.SendAsync(request);

            return Ok(request.Result);
        }

        [Authorize]
        [HttpPost("/api/dictionaries", Name = "CreateDictionary")]
        [Produces(typeof(DictionaryView))]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody]DictionaryView value)
        {
            var request = new PostDictionaryRequest { Dictionary = value };
            await _commandProcessor.SendAsync(request);

            return Created(request.Result.Location, request.Result.Response);
        }

        [Authorize]
        [HttpPut("/api/dictionaries/{id}", Name = "UpdateDictionary")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, [FromBody] DictionaryView value)
        {
            var request = new PutDictionaryRequest(id) { Dictionary = value };
            await _commandProcessor.SendAsync(request);

            if (request.Result.Response != null)
            {
                return Created(request.Result.Location, request.Result.Response);
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("/api/dictionaries/{id}", Name = "DeleteDictionary")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = new DeleteDictionaryRequest(id);
            await _commandProcessor.SendAsync(request);
            return NoContent();
        }
    }
}