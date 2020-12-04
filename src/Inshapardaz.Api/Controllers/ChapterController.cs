using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;
using System.IO;
using System.Text;
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

        [HttpGet("libraries/{libraryId}/books/{bookId}/chapters/{chapterId}", Name = nameof(ChapterController.GetChapterById))]
        public async Task<IActionResult> GetChapterById(int libraryId, int bookId, int chapterId, CancellationToken token = default(CancellationToken))
        {
            var query = new GetChapterByIdQuery(libraryId, bookId, chapterId);
            var chapter = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (chapter != null)
            {
                return new OkObjectResult(_chapterRenderer.Render(chapter, libraryId, bookId));
            }

            return new NotFoundResult();
        }

        [HttpPost("libraries/{libraryId}/books/{bookId}/chapters", Name = nameof(ChapterController.CreateChapter))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> CreateChapter(int libraryId, int bookId, [FromBody]ChapterView chapter, CancellationToken token = default(CancellationToken))
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

        [HttpPut("libraries/{libraryId}/books/{bookId}/chapters/{chapterId}", Name = nameof(ChapterController.UpdateChapter))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateChapter(int libraryId, int bookId, int chapterId, [FromBody]ChapterView chapter, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new UpdateChapterRequest(libraryId, bookId, chapterId, chapter.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _chapterRenderer.Render(request.Result.Chapter, libraryId, bookId);

            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new OkObjectResult(renderResult);
        }

        [HttpDelete("libraries/{libraryId}/books/{bookId}/chapters/{chapterId}", Name = nameof(ChapterController.DeleteChapter))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeleteChapter(int libraryId, int bookId, int chapterId, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteChapterRequest(libraryId, bookId, chapterId);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpGet("libraries/{libraryId}/books/{bookId}/chapters/{chapterId}/contents", Name = nameof(ChapterController.GetChapterContent))]
        [Authorize()]
        public async Task<IActionResult> GetChapterContent(int libraryId, int bookId, int chapterId, CancellationToken token = default(CancellationToken))
        {
            var contentType = Request.Headers["Accept"]; // default to "text/markdown"
            var language = Request.Headers["Accept-Language"]; // default to  ""

            var query = new GetChapterContentQuery(libraryId, bookId, chapterId, language, contentType, _userHelper.Account?.Id);

            var chapterContents = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (chapterContents != null)
            {
                return new OkObjectResult(_chapterRenderer.Render(chapterContents, libraryId));
            }

            return new NotFoundResult();
        }

        [HttpPost("libraries/{libraryId}/books/{bookId}/chapters/{chapterId}/contents", Name = nameof(ChapterController.CreateChapterContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> CreateChapterContent(int libraryId, int bookId, int chapterId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }
            var language = Request.Headers["Accept-Language"];

            var request = new AddChapterContentRequest(libraryId, bookId, chapterId, Encoding.Default.GetString(content), language, file.ContentType);
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                var renderResult = _chapterRenderer.Render(request.Result, libraryId);
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new BadRequestResult();
        }

        [HttpPut("libraries/{libraryId}/books/{bookId}/chapters/{chapterId}/contents", Name = nameof(ChapterController.UpdateChapterContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateChapterContent(int libraryId, int bookId, int chapterId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var language = Request.Headers["Accept-Language"];

            var request = new UpdateChapterContentRequest(libraryId, bookId, chapterId, Encoding.Default.GetString(content), language, file.ContentType);
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _chapterRenderer.Render(request.Result.ChapterContent, libraryId);

            if (request.Result != null && request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new OkObjectResult(renderResult);
        }

        [HttpDelete("libraries/{libraryId}/books/{bookId}/chapters/{chapterId}/contents", Name = nameof(ChapterController.DeleteChapterContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeleteChapterContent(int libraryId, int bookId, int chapterId, CancellationToken token = default(CancellationToken))
        {
            var contentType = Request.Headers["Accept"];
            var language = Request.Headers["Accept-Language"];

            var request = new DeleteChapterContentRequest(libraryId, bookId, chapterId, language, contentType);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }
    }
}
