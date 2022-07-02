using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Controllers
{
    public class ChapterController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderChapter _chapterRenderer;
        private readonly IUserHelper _userHelper;

        public ChapterController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderChapter chapterRenderer,
            IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _chapterRenderer = chapterRenderer;
            _userHelper = userHelper;
        }

        [HttpGet("libraries/{libraryId}/books/{bookId}/chapters", Name = nameof(ChapterController.GetChaptersByBook))]
        [Produces(typeof(ListView<ChapterView>))]
        public async Task<IActionResult> GetChaptersByBook(int libraryId, int bookId, CancellationToken token = default(CancellationToken))
        {
            var query = new GetChaptersByBookQuery(libraryId, bookId, _userHelper.Account?.Id);
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
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> CreateChapter(int libraryId, int bookId, [FromBody] ChapterView chapter, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AddChapterRequest(libraryId, bookId, _userHelper.Account?.Id, chapter.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                var renderResult = _chapterRenderer.Render(request.Result, libraryId, bookId);
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new BadRequestResult();
        }

        [HttpPut("libraries/{libraryId}/books/{bookId}/chapters/{chapterNumber}", Name = nameof(ChapterController.UpdateChapter))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
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
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeleteChapter(int libraryId, int bookId, int chapterNumber, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteChapterRequest(libraryId, bookId, chapterNumber);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPost("libraries/{libraryId}/books/{bookId}/chapters/sequence", Name = nameof(ChapterController.UpdateChapterSequence))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
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
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        [Produces(typeof(BookPageView))]
        public async Task<IActionResult> AssignChapterToUser(int libraryId, int bookId, int chapterNumber, [FromBody] ChapterAssignmentView assignment, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AssignChapterToUserRequest(libraryId, bookId, chapterNumber, assignment.AccountId ?? _userHelper.Account.Id, _userHelper.IsAdmin);

            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _chapterRenderer.Render(request.Result, libraryId, bookId);

            return Ok(renderResult);
        }


        [HttpGet("libraries/{libraryId}/books/{bookId}/chapters/{chapterNumber}/contents", Name = nameof(ChapterController.GetChapterContent))]
        [Authorize()]
        [Produces(typeof(ChapterContentView))]
        public async Task<IActionResult> GetChapterContent(int libraryId, int bookId, int chapterNumber, string language, CancellationToken token = default(CancellationToken))
        {
            //var language = Request.Headers["Accept-Language"];

            var query = new GetChapterContentQuery(libraryId, bookId, chapterNumber, language, _userHelper.Account?.Id);

            var chapterContents = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (chapterContents != null)
            {
                return new OkObjectResult(_chapterRenderer.Render(chapterContents, libraryId));
            }

            return new NotFoundResult();
        }

        [HttpPost("libraries/{libraryId}/books/{bookId}/chapters/{chapterNumber}/contents", Name = nameof(ChapterController.CreateChapterContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> CreateChapterContent(int libraryId, int bookId, int chapterNumber, string language, [FromBody] string content, CancellationToken token = default(CancellationToken))
        {
            //var language = Request.Headers["Content-Language"];

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
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateChapterContent(int libraryId, int bookId, int chapterNumber, string language, [FromBody] string content, CancellationToken token = default(CancellationToken))
        {
            //var language = Request.Headers["Content-Language"];

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
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeleteChapterContent(int libraryId, int bookId, int chapterNumber, string language, CancellationToken token = default(CancellationToken))
        {
            //var language = Request.Headers["Accept-Language"];

            var request = new DeleteChapterContentRequest(libraryId, bookId, chapterNumber, language);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }
    }
}
