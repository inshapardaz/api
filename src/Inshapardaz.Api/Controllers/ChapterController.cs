using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;
using Inshapardaz.Domain.Ports.Query.Library.Book.Chapter;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

public class ChapterController : Controller
{
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IRenderChapter _chapterRenderer;

    public ChapterController(IAmACommandProcessor commandProcessor,
        IQueryProcessor queryProcessor,
        IRenderChapter chapterRenderer)
    {
        _commandProcessor = commandProcessor;
        _queryProcessor = queryProcessor;
        _chapterRenderer = chapterRenderer;
    }

    [HttpGet("libraries/{libraryId}/books/{bookId}/chapters", Name = nameof(ChapterController.GetChaptersByBook))]
    [Produces(typeof(ListView<ChapterView>))]
    public async Task<IActionResult> GetChaptersByBook(int libraryId, int bookId, CancellationToken token = default(CancellationToken))
    {
        var query = new GetChaptersByBookQuery(libraryId, bookId);
        var chapters = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

        if (chapters != null)
        {
            return new OkObjectResult(_chapterRenderer.Render(chapters, libraryId, bookId));
        }

        return new NotFoundResult();
    }

    [HttpGet("libraries/{libraryId}/books/{bookId}/chapters/{chapterNumber}", Name = nameof(ChapterController.GetChapterById))]
    [Produces(typeof(ChapterView))]
    public async Task<IActionResult> GetChapterById(int libraryId, int bookId, int chapterNumber, CancellationToken token = default(CancellationToken))
    {
        var query = new GetChapterByIdQuery(libraryId, bookId, chapterNumber);
        var chapter = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

        if (chapter != null)
        {
            return new OkObjectResult(_chapterRenderer.Render(chapter, libraryId, bookId));
        }

        return new NotFoundResult();
    }

    [HttpPost("libraries/{libraryId}/books/{bookId}/chapters", Name = nameof(ChapterController.CreateChapter))]
    public async Task<IActionResult> CreateChapter(int libraryId, int bookId, [FromBody] ChapterView chapter, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new AddChapterRequest(libraryId, bookId, chapter.Map());
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        if (request.Result != null)
        {
            var renderResult = _chapterRenderer.Render(request.Result, libraryId, bookId);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        return new BadRequestResult();
    }

    [HttpPut("libraries/{libraryId}/books/{bookId}/chapters/{chapterNumber}", Name = nameof(ChapterController.UpdateChapter))]
    public async Task<IActionResult> UpdateChapter(int libraryId, int bookId, int chapterNumber, [FromBody] ChapterView chapter, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new UpdateChapterRequest(libraryId, bookId, chapterNumber, chapter.Map());
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = _chapterRenderer.Render(request.Result.Chapter, libraryId, bookId);

        if (request.Result.HasAddedNew)
        {
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        return new OkObjectResult(renderResult);
    }

    [HttpDelete("libraries/{libraryId}/books/{bookId}/chapters/{chapterNumber}", Name = nameof(ChapterController.DeleteChapter))]
    public async Task<IActionResult> DeleteChapter(int libraryId, int bookId, int chapterNumber, CancellationToken token = default(CancellationToken))
    {
        var request = new DeleteChapterRequest(libraryId, bookId, chapterNumber);
        await _commandProcessor.SendAsync(request, cancellationToken: token);
        return new NoContentResult();
    }

    [HttpPost("libraries/{libraryId}/books/{bookId}/chapters/sequence", Name = nameof(ChapterController.UpdateChapterSequence))]
    public async Task<IActionResult> UpdateChapterSequence(int libraryId, int bookId, [FromBody] IEnumerable<ChapterView> chapters, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new UpdateChapterSequenceRequest(libraryId, bookId, chapters.Select(c => c.Map()));
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        return new OkObjectResult(_chapterRenderer.Render(request.Result, libraryId, bookId));
    }

    [HttpPost("libraries/{libraryId}/books/{bookId}/chapters/{chapterNumber}/assign", Name = nameof(ChapterController.AssignChapterToUser))]
    [Produces(typeof(BookPageView))]
    public async Task<IActionResult> AssignChapterToUser(int libraryId, int bookId, int chapterNumber, [FromBody] AssignmentView assignment, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new AssignChapterToUserRequest(libraryId, bookId, chapterNumber, assignment.Unassign ? null : assignment.AccountId);

        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = _chapterRenderer.Render(request.Result, libraryId, bookId);

        return Ok(renderResult);
    }


    [HttpGet("libraries/{libraryId}/books/{bookId}/chapters/{chapterNumber}/contents", Name = nameof(ChapterController.GetChapterContent))]
    [Produces(typeof(ChapterContentView))]
    public async Task<IActionResult> GetChapterContent(int libraryId, int bookId, int chapterNumber, [FromQuery] string language, CancellationToken token = default(CancellationToken))
    {
        var query = new GetChapterContentQuery(libraryId, bookId, chapterNumber, language);

        var chapterContents = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

        if (chapterContents != null)
        {
            return new OkObjectResult(_chapterRenderer.Render(chapterContents, libraryId));
        }

        return new NotFoundResult();
    }

    [HttpPost("libraries/{libraryId}/books/{bookId}/chapters/{chapterNumber}/contents", Name = nameof(ChapterController.CreateChapterContent))]
    public async Task<IActionResult> CreateChapterContent(int libraryId, int bookId, int chapterNumber, [FromQuery] string language, [FromBody] string content, CancellationToken token = default(CancellationToken))
    {
        var request = new AddChapterContentRequest(libraryId, bookId, chapterNumber, content, language);
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        if (request.Result != null)
        {
            var renderResult = _chapterRenderer.Render(request.Result, libraryId);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        return new BadRequestResult();
    }

    [HttpPut("libraries/{libraryId}/books/{bookId}/chapters/{chapterNumber}/contents", Name = nameof(ChapterController.UpdateChapterContent))]
    public async Task<IActionResult> UpdateChapterContent(int libraryId, int bookId, int chapterNumber, [FromQuery] string language, [FromBody] string content, CancellationToken token = default(CancellationToken))
    {
        var request = new UpdateChapterContentRequest(libraryId, bookId, chapterNumber, content, language);
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = _chapterRenderer.Render(request.Result.ChapterContent, libraryId);

        if (request.Result != null && request.Result.HasAddedNew)
        {
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        return new OkObjectResult(renderResult);
    }

    [HttpDelete("libraries/{libraryId}/books/{bookId}/chapters/{chapterNumber}/contents", Name = nameof(ChapterController.DeleteChapterContent))]
    public async Task<IActionResult> DeleteChapterContent(int libraryId, int bookId, int chapterNumber, [FromQuery] string language, CancellationToken token = default(CancellationToken))
    {
        var request = new DeleteChapterContentRequest(libraryId, bookId, chapterNumber, language);
        await _commandProcessor.SendAsync(request, cancellationToken: token);
        return new NoContentResult();
    }
}
