using System.Threading.Tasks;
using Inshapardaz.Api.Adapters.Dictionary;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers
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
            var request = new GetDictionaryByIdRequest { DictionaryId = id };
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
            var request = new PutDictionaryRequest { Dictionary = value };
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
            var request = new DeleteDictionaryRequest {DictionaryId = id};
            await _commandProcessor.SendAsync(request);
            return NoContent();
        }

        [Authorize]
        [HttpPost("/api/dictionaries/{id}/download", Name = "CreateDictionaryDownload")]
        [Produces(typeof(DownloadDictionaryView))]
        public async Task<IActionResult> CreateDownloadForDictionary(int id)
        {
            var request = new CreateDictionaryDownloadRequest { DictionaryId = id };
            await _commandProcessor.SendAsync(request);
            
            return Created(request.Result.Location, request.Result.Response);
        }

        [HttpGet("/api/dictionary/{id}/download", Name = "DownloadDictionary")]
        [Produces(typeof(byte[]))]
        public async Task<IActionResult> DownloadDictionary(int id, [FromHeader(Name = "Accept")] string accept = MimeTypes.SqlLite)
        {

            var request = new DownloadDictionaryRequest {DictionaryId = id, MimeType = accept};
            await _commandProcessor.SendAsync(request);
            
            return File(request.Result.Contents, accept, request.Result.FileName);
        }
    }
}