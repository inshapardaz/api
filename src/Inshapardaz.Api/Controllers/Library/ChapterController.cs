using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers.Library;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Ports.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers.Library
{
    public class ChapterController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderChapters _chaptersRenderer;
        private readonly IRenderChapter _renderChapter;
        private readonly IRenderChapterContent _chapterContentRender;

        public ChapterController(IAmACommandProcessor commandProcessor, IUserHelper userHelper, IRenderChapters chaptersRenderer, IRenderChapter renderChapter, IRenderChapterContent chapterContentRender)
        {
            _commandProcessor = commandProcessor;
            _userHelper = userHelper;
            _chaptersRenderer = chaptersRenderer;
            _renderChapter = renderChapter;
            _chapterContentRender = chapterContentRender;
        }

        [HttpGet("/api/books/{bookId}/chapters", Name = "GetChaptersForBook")]
        [Produces(typeof(ListView<ChapterView>))]
        public async Task<IActionResult> GetChaptersByBook(int bookId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new GetChaptersByBookRequest(bookId) { UserId = _userHelper.GetUserId() };
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result != null)
            {
                return Ok(_chaptersRenderer.Render(bookId, request.Result));
            }

            return NotFound();
        }


        [HttpGet("/api/books/{bookId}/chapters/{chapterId}", Name = "GetChapterById")]
        [Produces(typeof(ChapterView))]
        public async Task<IActionResult> GetBooksById(int bookId, int chapterId, CancellationToken cancellationToken)
        {
            var request = new GetChapterByIdRequest(bookId, chapterId);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result != null)
            {
                return Ok(_renderChapter.Render(request.Result));
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost("/api/books/{bookId}/chapters", Name = "CreateChapter")]
        [Produces(typeof(ChapterView))]
        [ValidateModel]
        public async Task<IActionResult> Post(int bookId, [FromBody]ChapterView value, CancellationToken cancellationToken)
        {
            var request = new AddChapterRequest(bookId, value.Map<ChapterView, Chapter>());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var renderResult = _renderChapter.Render(request.Result);
            return Created(renderResult.Links.Self(), renderResult);
        }

        [Authorize]
        [HttpPut("/api/books/{bookId}/chapters/{chapterId}", Name = "UpdateChapter")]
        [ValidateModel]
        public async Task<IActionResult> Put(int bookId, int chapterId, [FromBody] ChapterView value, CancellationToken cancellationToken)
        {
            var request = new UpdateChapterRequest(bookId, value.Map<ChapterView, Chapter>());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result.HasAddedNew)
            {
                var renderResult = _renderChapter.Render(request.Result.Chapter);
                return Created(renderResult.Links.Self(), renderResult);
            }

            return NoContent();
        }

        [HttpGet("/api/books/{bookId}/chapters/{chapterId}/contents", Name = "GetChapterContents")]
        [ValidateModel]
        public async Task<IActionResult> GetChapterContents(int bookId, int chapterId, CancellationToken cancellationToken)
        {
            var request = new GetChapterContentRequest(bookId, chapterId);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result == null)
                return NotFound();
            return Ok(_chapterContentRender.Render(request.Result));
        }

        [Authorize]
        [HttpPost("/api/books/{bookId}/chapters/{chapterId}/contents", Name = "AddChapterContents")]
        [ValidateModel]
        public async Task<IActionResult> PostContents(int bookId, int chapterId, [FromBody] string content, CancellationToken cancellationToken)
        {
            var request = new AddChapterContentRequest(new ChapterContent
            {
                BookId = bookId,
                ChapterId = chapterId,
                Content = content
            });
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            return NoContent();
        }

        [Authorize]
        [HttpPut("/api/books/{bookId}/chapters/{chapterId}/contents", Name = "UpdateChapterContents")]
        [ValidateModel]
        public async Task<IActionResult> Put(int bookId, int chapterId, [FromBody] string content, CancellationToken cancellationToken)
        {
                var request = new UpdateChapterContentRequest(new ChapterContent
            {
                BookId = bookId,
                ChapterId = chapterId,
                Content = content
            });
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result.HasAddedNew)
            {
                var renderResult = _chapterContentRender.Render(request.Result.ChapterContent);
                return Created(renderResult.Links.Self(), renderResult);
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("/api/books/{bookId}/chapters/{chapterId}", Name = "DeleteChapter")]
        public async Task<IActionResult> Delete(int bookId, int chapterId, CancellationToken cancellationToken)
        {
            var request = new DeleteChapterRequest(bookId, chapterId);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);
            return NoContent();
        }

    }
}
