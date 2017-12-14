using System.Threading.Tasks;
using Inshapardaz.Api.Adapters.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/DictionaryDownload")]
    public class DictionaryDownloadController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;

        public DictionaryDownloadController(IAmACommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        [HttpGet("/api/dictionary/{id}/download", Name = "DownloadDictionary")]
        [Produces(typeof(byte[]))]
        public async Task<IActionResult> DownloadDictionary(int id, [FromHeader(Name = "Accept")] string accept = MimeTypes.SqlLite)
        {

            var request = new DownloadDictionaryRequest(id) { MimeType = accept };
            await _commandProcessor.SendAsync(request);

            return File(request.Result.Contents, accept, request.Result.FileName);
        }

        [Authorize]
        [HttpPost("/api/dictionaries/{id}/download", Name = "CreateDictionaryDownload")]
        [Produces(typeof(DownloadDictionaryView))]
        public async Task<IActionResult> CreateDownloadForDictionary(int id)
        {
            var request = new CreateDictionaryDownloadRequest(id);
            await _commandProcessor.SendAsync(request);

            return Created(request.Result.Location, request.Result.Response);
        }
    }
}