using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers
{
    public class BookPageController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderBookPage _bookPageRenderer;
        private readonly IUserHelper _userHelper;
        private IRenderFile _fileRenderer;

        public BookPageController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderBookPage bookPageRenderer,
            IRenderFile fileRenderer,
            IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _bookPageRenderer = bookPageRenderer;
            _fileRenderer = fileRenderer;
            _userHelper = userHelper;
        }

        [HttpGet("library/{libraryId}/books/{bookId}/pages", Name = nameof(BookPageController.GetPagesByBook))]
        public async Task<IActionResult> GetPagesByBook(int libraryId, int bookId, int pageNumber = 1,
            int pageSize = 10, CancellationToken token = default(CancellationToken))
        {
            var authorsQuery = new GetBookPagesQuery(libraryId, bookId, pageNumber, pageSize);
            var result = await _queryProcessor.ExecuteAsync(authorsQuery, token);

            var args = new PageRendererArgs<BookPageModel>
            {
                Page = result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
            };

            return new OkObjectResult(_bookPageRenderer.Render(args, libraryId, bookId));
        }

        [HttpGet("library/{libraryId}/books/{bookId}/pages/{sequenceNumber}", Name = nameof(BookPageController.GetPagesByIndex))]
        public async Task<IActionResult> GetPagesByIndex(int libraryId, int bookId, int sequenceNumber, CancellationToken token = default(CancellationToken))
        {
            var request = new GetBookPageByNumberQuery(libraryId, bookId, sequenceNumber);

            var result = await _queryProcessor.ExecuteAsync(request, cancellationToken: token);

            if (result == null)
            {
                return NotFound();
            }

            var renderResult = _bookPageRenderer.Render(result, libraryId);
            return Ok(renderResult);
        }

        [HttpPost("library/{libraryId}/books/{bookId}/pages", Name = nameof(BookPageController.CreatePage))]
        public async Task<IActionResult> CreatePage(int libraryId, int bookId, [FromBody]BookPageView page, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var model = page.Map();

            var request = new AddBookPageRequest(_userHelper.Claims, libraryId, bookId, model);

            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _bookPageRenderer.Render(request.Result, libraryId);

            //TODO: Remove updation
            if (request.IsAdded)
            {
                return Created(renderResult.Links.Self(), renderResult);
            }

            return Ok(renderResult);
        }

        [HttpPut("library/{libraryId}/books/{bookId}/pages/{sequenceNumber}", Name = nameof(BookPageController.UpdatePage))]
        public async Task<IActionResult> UpdatePage(int libraryId, int bookId, int sequenceNumber, [FromBody]BookPageView page, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var model = page.Map();

            var request = new UpdateBookPageRequest(_userHelper.Claims, libraryId, bookId, sequenceNumber, model);

            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _bookPageRenderer.Render(request.Result.BookPage, libraryId);

            if (request.Result.HasAddedNew)
            {
                return Created(renderResult.Links.Self(), renderResult);
            }

            return Ok(renderResult);
        }

        [HttpDelete("library/{libraryId}/books/{bookId}/pages/{sequenceNumber}", Name = nameof(BookPageController.DeletePage))]
        public async Task<IActionResult> DeletePage(int libraryId, int bookId, int sequenceNumber, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteBookPageRequest(_userHelper.Claims, libraryId, bookId, sequenceNumber);
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            return Ok();
        }

        [HttpPut("library/{libraryId}/books/{bookId}/pages/{sequenceNumber}/image", Name = nameof(BookPageController.UpdatePageImage))]
        public async Task<IActionResult> UpdatePageImage(int libraryId, int bookId, int sequenceNumber, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var request = new UpdateBookPageImageRequest(_userHelper.Claims, libraryId, bookId, sequenceNumber)
            {
                Image = new FileModel
                {
                    FileName = file.FileName,
                    MimeType = file.ContentType,
                    Contents = content
                }
            };

            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result.HasAddedNew)
            {
                var imageLink = _bookPageRenderer.RenderImageLink(request.Result.File.Id);

                return new CreatedResult(imageLink.Href, null);
            }

            return new OkResult();
        }

        [HttpDelete("library/{libraryId}/books/{bookId}/pages/{sequenceNumber}/image", Name = nameof(BookPageController.DeletePageImage))]
        public async Task<IActionResult> DeletePageImage(int libraryId, int bookId, int sequenceNumber, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteBookPageImageRequest(_userHelper.Claims, libraryId, bookId, sequenceNumber);
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            return Ok();
        }

        [HttpPost("library/{libraryId}/books/{bookId}/pages/{sequenceNumber}/assign", Name = nameof(BookPageController.AssignPage))]
        public async Task<IActionResult> AssignPage(int libraryId, int bookId, int sequenceNumber, [FromBody]BookPageAssignmentView assignment, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AssignBookPageRequest(_userHelper.Claims, libraryId, bookId, sequenceNumber, assignment.Status, assignment.UserId);

            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _bookPageRenderer.Render(request.Result, libraryId);

            return Ok(renderResult);
        }
    }
}
