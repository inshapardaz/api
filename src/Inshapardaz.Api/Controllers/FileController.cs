using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers
{
    public class FileController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        public FileController(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        [HttpGet("files/{fileId}", Name = nameof(FileController.GetFile))]
        public async Task<IActionResult> GetFile(int fileId, CancellationToken token = default(CancellationToken))
        {
            var query = new GetFileQuery(fileId, 200, 200);
            var file = await _queryProcessor.ExecuteAsync(query, token);

            if (file == null)
            {
                return new NotFoundResult();
            }

            return File(file.Contents, file.MimeType);
        }

        [HttpDelete("files/{fileId}", Name = nameof(FileController.DeleteFile))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeleteFile(int fileId, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteFileRequest(fileId);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }
    }
}
