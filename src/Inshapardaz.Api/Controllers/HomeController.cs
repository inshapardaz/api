using Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;
using Inshapardaz.Domain.Ports.Query.Library.Book.Chapter;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

public class HomeController : Controller
{
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;

    public HomeController(IAmACommandProcessor commandProcessor,
        IQueryProcessor queryProcessor)
    {
        _commandProcessor = commandProcessor;
        _queryProcessor = queryProcessor;
    }

    [HttpGet("health/check")]
    public IActionResult GetHeathCheck()
    {
        return Ok();
    }

    [HttpPost("/libraries/{libraryId}/books/{bookId}/migrateChapters")]
    public async Task<IActionResult> MigrateBook(int libraryId, int bookId, CancellationToken token)
    {
        var query = new GetChaptersByBookQuery(libraryId, bookId);
        var chapters = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);
        foreach (var chapter in chapters)
        {
            foreach (var content in chapter.Contents)
            {
                var chapterContentQuery = new GetChapterContentQuery(libraryId, bookId, chapter.ChapterNumber, content.Language);

                var chapterContents = await _queryProcessor.ExecuteAsync(chapterContentQuery, cancellationToken: token);

                if (chapterContents != null && chapterContents.FileId == null)
                {
                    var request = new UpdateChapterContentRequest(libraryId, bookId,
                        chapter.ChapterNumber, chapterContents.Text, chapterContents.Language);
                    await _commandProcessor.SendAsync(request, cancellationToken: token);
                }
            }
        }

        return Ok("Migrated chapters successfully.");
    }
}

