using System.Threading.Tasks;
using Inshapardaz.Api.Adapters.Dictionary;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Ports.Dictionary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers.Dictionary
{
    [Produces("application/json")]
    [Route("api/DictionaryDownload")]
    public class DictionaryDownloadController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderDictionaryDownload _dictionaryDownload;

        public DictionaryDownloadController(IAmACommandProcessor commandProcessor, IRenderDictionaryDownload dictionaryDownload)
        {
            _commandProcessor = commandProcessor;
            _dictionaryDownload = dictionaryDownload;
        }

        [HttpGet("/api/dictionary/{id}/download", Name = "DownloadDictionary")]
        [Produces(typeof(byte[]))]
        public async Task<IActionResult> DownloadDictionary(int id, [FromHeader(Name = "Accept")] string accept = MimeTypes.SqlLite)
        {

            var request = new GetDownloadDictionaryRequest(id) { MimeType = accept };
            await _commandProcessor.SendAsync(request);

            return File(request.Result.Contents, accept, request.Result.FileName);
        }

        [Authorize]
        [HttpPost("/api/dictionaries/{id}/download", Name = "CreateDictionaryDownload")]
        [Produces(typeof(DownloadDictionaryView))]
        public async Task<IActionResult> CreateDownloadForDictionary(int id)
        {
            //var request = new PostDictionaryDownloadRequest(id);
            //await _commandProcessor.SendAsync(request);

            //return Created(_dictionaryDownload.Render(new DownloadJobModel(){}}) request.Result));
            return NotFound();
        }
    }
}