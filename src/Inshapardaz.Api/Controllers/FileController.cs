using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers
{
    public class FileController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;

        public FileController(IAmACommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        [HttpGet("/api/file/{id}", Name = "GetFileById")]
        public async Task<IActionResult> GetFile(int id, int height = 200, int width = 200, CancellationToken cancellationToken = new CancellationToken())
        {
            var request = new GetFileRequest(id, height, width);
            await _commandProcessor.SendAsync(request, false,  cancellationToken);

            if (request.Response == null)
            {
                return NotFound();
            }

            return File(request.Response.Contents, request.Response.MimeType, request.Response.FileName);
        }

        [Authorize]
        [HttpDelete("/api/file/{id}", Name = "DeleteFile")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var request = new DeleteFileRequest(id);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);
            return NoContent();
        }
    }
}
