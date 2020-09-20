using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
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

        [HttpGet("library/{libraryId}/books/{bookId}/chapters", Name = nameof(ChapterController.GetChaptersByBook))]
        public async Task<IActionResult> GetChaptersByBook(int libraryId, int bookId, CancellationToken token = default(CancellationToken))
        {
            var query = new GetChaptersByBookQuery(libraryId, bookId, _userHelper.GetUserId());
            var chapters = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (chapters != null)
            {
                return new OkObjectResult(_chapterRenderer.Render(chapters, libraryId, bookId));
            }

            return new NotFoundResult();
        }

        [HttpGet("library/{libraryId}/books/{bookId}/chapters/{chapterId}", Name = nameof(ChapterController.GetChapterById))]
        public async Task<IActionResult> GetChapterById(int libraryId, int bookId, int chapterId, CancellationToken token = default(CancellationToken))
        {
            var query = new GetChapterByIdQuery(libraryId, bookId, chapterId, _userHelper.GetUserId());
            var chapter = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (chapter != null)
            {
                return new OkObjectResult(_chapterRenderer.Render(chapter, libraryId, bookId));
            }

            return new NotFoundResult();
        }

        [HttpPost("library/{libraryId}/books/{bookId}/chapters", Name = nameof(ChapterController.CreateChapter))]
        public async Task<IActionResult> CreateChapter(int libraryId, int bookId, [FromBody]ChapterView chapter, CancellationToken token = default(CancellationToken))
        {
            var request = new AddChapterRequest(_userHelper.Claims, libraryId, bookId, chapter.Map(), _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                var renderResult = _chapterRenderer.Render(request.Result, libraryId, bookId);
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new BadRequestResult();
        }

        [HttpPut("library/{libraryId}/books/{bookId}/chapters/{chapterId}", Name = nameof(ChapterController.UpdateChapter))]
        public async Task<IActionResult> UpdateChapter(int libraryId, int bookId, int chapterId, [FromBody]ChapterView chapter, CancellationToken token = default(CancellationToken))
        {
            var request = new UpdateChapterRequest(_userHelper.Claims, libraryId, bookId, chapterId, chapter.Map(), _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _chapterRenderer.Render(request.Result.Chapter, libraryId, bookId);

            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new OkObjectResult(renderResult);
        }

        [HttpDelete("library/{libraryId}/books/{bookId}/chapters/{chapterId}", Name = nameof(ChapterController.DeleteChapter))]
        public async Task<IActionResult> DeleteChapter(int libraryId, int bookId, int chapterId, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteChapterRequest(_userHelper.Claims, libraryId, bookId, chapterId, _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpGet("library/{libraryId}/books/{bookId}/chapters/{chapterId}/contents", Name = nameof(ChapterController.GetChapterContent))]
        public async Task<IActionResult> GetChapterContent(int libraryId, int bookId, int chapterId, CancellationToken token = default(CancellationToken))
        {
            var contentType = Request.Headers["Accept"]; // default to "text/markdown"
            var language = Request.Headers["Accept-Language"]; // default to  ""

            var query = new GetChapterContentQuery(libraryId, bookId, chapterId, language, contentType, _userHelper.GetUserId());

            var chapterContents = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (chapterContents != null)
            {
                return new OkObjectResult(_chapterRenderer.Render(chapterContents, libraryId));
            }

            return new NotFoundResult();
        }

        [HttpPost("library/{libraryId}/books/{bookId}/chapters/{chapterId}/contents", Name = nameof(ChapterController.CreateChapterContent))]
        public async Task<IActionResult> CreateChapterContent(int libraryId, int bookId, int chapterId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }
            var language = Request.Headers["Accept-Language"];

            var request = new AddChapterContentRequest(_userHelper.Claims, libraryId, bookId, chapterId, Encoding.Default.GetString(content), language, file.ContentType, _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                var renderResult = _chapterRenderer.Render(request.Result, libraryId);
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new BadRequestResult();
        }

        [HttpPut("library/{libraryId}/books/{bookId}/chapters/{chapterId}/contents", Name = nameof(ChapterController.UpdateChapterContent))]
        public async Task<IActionResult> UpdateChapterContent(int libraryId, int bookId, int chapterId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var language = Request.Headers["Accept-Language"];

            var request = new UpdateChapterContentRequest(_userHelper.Claims, libraryId, bookId, chapterId, Encoding.Default.GetString(content), language, file.ContentType, _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _chapterRenderer.Render(request.Result.ChapterContent, libraryId);

            if (request.Result != null && request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new OkObjectResult(renderResult);
        }

        [HttpDelete("library/{libraryId}/books/{bookId}/chapters/{chapterId}/contents", Name = nameof(ChapterController.DeleteChapterContent))]
        public async Task<IActionResult> DeleteChapterContent(int libraryId, int bookId, int chapterId, CancellationToken token = default(CancellationToken))
        {
            var contentType = Request.Headers["Accept"];
            var language = Request.Headers["Accept-Language"];

            var request = new DeleteChapterContentRequest(_userHelper.Claims, libraryId, bookId, chapterId, language, contentType, _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }
    }
}
