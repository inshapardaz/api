using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Domain.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers
{
    public class FileController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderFile _renderFile;

        public FileController(IAmACommandProcessor commandProcessor, IRenderFile renderFile)
        {
            _commandProcessor = commandProcessor;
            _renderFile = renderFile;
        }

        [HttpGet("/api/file/{id}.{ext?}", Name = "GetFileById")]
        public async Task<IActionResult> GetFile(int id, string ext, int height = 200, int width = 200, CancellationToken cancellationToken = new CancellationToken())
        {
            var request = new GetFileRequest(id, height, width);
            await _commandProcessor.SendAsync(request, false,  cancellationToken);

            if (request.Response == null)
            {
                return NotFound();
            }

            return File(request.Response.Contents, request.Response.MimeType, request.Response.FileName);
        }

        //[HttpPost("/api/file", Name = "CreateFie")]
        //public async Task<IActionResult> CreateFile(IFormFile file, CancellationToken cancellationToken = new CancellationToken())
        //{
        //    var content = new byte[file.Length];
        //    using (var stream = new MemoryStream(content))
        //    {
        //        await file.CopyToAsync(stream, cancellationToken);
        //    }

        //    var request = new AddFileRequest(new Domain.Entities.File
        //    {
        //        FileName = file.FileName ?? Guid.NewGuid().ToString("N"),
        //        MimeType = file.ContentType,
        //        Contents = content
        //    });
        //    await _commandProcessor.SendAsync(request, false, cancellationToken);

        //    if (request.Response == null)
        //    {
        //        return NotFound();
        //    }

        //    var response = _renderFile.Render(request.Response);

        //    return Created(response.Links.Self(), response);
        //}

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
