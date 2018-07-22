using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Library;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Ports.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly IRenderFile _fileRenderer;

        public ChapterController(IAmACommandProcessor commandProcessor, IUserHelper userHelper, IRenderChapters chaptersRenderer, IRenderChapter renderChapter, IRenderFile fileRenderer)
        {
            _commandProcessor = commandProcessor;
            _userHelper = userHelper;
            _chaptersRenderer = chaptersRenderer;
            _renderChapter = renderChapter;
            _fileRenderer = fileRenderer;
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

        [Authorize]
        [HttpPost("/api/books/{bookId}/chapters/{chapterId}/contents", Name = "GetChapterContents")]
        [ValidateModel]
        public async Task<IActionResult> GetChapterContents(int bookId, int chapterId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        [HttpPost("/api/books/{bookId}/chapters/{chapterId}/contents", Name = "UpdateChapterContents")]
        [ValidateModel]
        public async Task<IActionResult> PutContents(int bookId, int chapterId, IFormFile file, CancellationToken cancellationToken)
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            //    var request = new UpdateBookImageRequest(id)
            //    {
            //        Image = new Domain.Entities.File
            //        {
            //            FileName = file.FileName,
            //            MimeType = file.ContentType,
            //            Contents = content
            //        }
            //    };

            //    await _commandProcessor.SendAsync(request);


            //    if (request.Result.HasAddedNew)
            //    {
            //        var response = _fileRenderer.Render(request.Result.File);
            //        return Created(response.Links.Self(), response);
            //    }

            return BadRequest();
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
