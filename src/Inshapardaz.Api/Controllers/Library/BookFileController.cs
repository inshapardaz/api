using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Library;
using Inshapardaz.Domain.Ports.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers.Library
{
    public class BookFileController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderFile _fileRenderer;
        private readonly IRenderBookFiles _bookFilesRenderer;

        public BookFileController(IAmACommandProcessor commandProcessor, IRenderFile fileRenderer, IRenderBookFiles bookFilesRenderer)
        {
            _commandProcessor = commandProcessor;
            _fileRenderer = fileRenderer;
            _bookFilesRenderer = bookFilesRenderer;
        }

        [Authorize]
        [HttpGet("/api/boos/{id}/files", Name = "GetBookFiles")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var command = new GetFilesByBookRequest(id);
            await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
            return Ok(_bookFilesRenderer.Render(id, command.Result));
        }

        [Authorize]
        [HttpPost("/api/books/{id}/files", Name = "AddBookFile")]
        [ValidateModel]
        public async Task<IActionResult> PostContent(int id, IFormFile file, CancellationToken cancellationToken)
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var request = new AddBookFileRequest(id)
            {
                Content = new Domain.Entities.File
                {
                    FileName = file.FileName,
                    MimeType = file.ContentType,
                    Contents = content
                }
            };

            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);


            var response = _fileRenderer.Render(request.Result);
            return Created(response.Links.Self(), response);
        }


        [Authorize]
        [HttpPut("/api/books/{id}/files/{fileId}", Name = "UpdateBookFile")]
        [ValidateModel]
        public async Task<IActionResult> PutContent(int id, int fileId, IFormFile file, CancellationToken cancellationToken)
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var request = new UpdateBookFileRequest(id, fileId)
            {
                Content = new Domain.Entities.File
                {
                    FileName = file.FileName,
                    MimeType = file.ContentType,
                    Contents = content
                }
            };

            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);


            if (request.Result.HasAddedNew)
            {
                var response = _fileRenderer.Render(request.Result.Content);
                return Created(response.Links.Self(), response);
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("/api/books/{id}/files/{fileId}", Name = "DeleteBookFile")]
        public async Task<IActionResult> Delete(int id, int fileId, CancellationToken cancellationToken)
        {
            var request = new DeleteBookFileRequest(id, fileId);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);
            return NoContent();
        }
    }
}