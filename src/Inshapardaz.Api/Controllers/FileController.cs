using Inshapardaz.Domain.Ports.Command.File;
using Inshapardaz.Domain.Ports.Query.File;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

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
        var query = new GetFileQuery(fileId) { Height = 200, Width = 200 };
        var file = await _queryProcessor.ExecuteAsync(query, token);

        if (file == null)
        {
            return new NotFoundResult();
        }

        return File(file.Contents, file.MimeType);
    }

    [HttpGet("libraries/{libraryId}/files/{fileId}", Name = nameof(FileController.GetLibraryFile))]
    public async Task<IActionResult> GetLibraryFile(int libraryId, int fileId, CancellationToken token = default(CancellationToken))
    {
        var query = new GetFileQuery(fileId) {  Height = 200, Width = 200 };
        var file = await _queryProcessor.ExecuteAsync(query, token);

        if (file == null)
        {
            return new NotFoundResult();
        }

        return File(file.Contents, file.MimeType);
    }

    [HttpDelete("files/{fileId}", Name = nameof(FileController.DeleteFile))]
    public async Task<IActionResult> DeleteFile(int fileId, CancellationToken token = default(CancellationToken))
    {
        var request = new DeleteFileCommand(fileId);
        await _commandProcessor.SendAsync(request, cancellationToken: token);
        return new NoContentResult();
    }
}
