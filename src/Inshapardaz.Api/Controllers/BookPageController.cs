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
            int pageSize = 10, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpGet("library/{libraryId}/books/{bookId}/pages/{pageNumber}", Name = nameof(BookPageController.GetPagesByIndex))]
        public async Task<IActionResult> GetPagesByIndex(int libraryId, int bookId, int pageNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpPost("library/{libraryId}/books/{bookId}/pages", Name = nameof(BookPageController.CreatePage))]
        public async Task<IActionResult> CreatePage(int libraryId, int bookId, [FromBody]BookPageView page, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var model = page.Map();
            model.BookId = bookId;

            var request = new AddBookPageRequest(_userHelper.Claims, libraryId, model);

            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var renderResult = _bookPageRenderer.Render(request.Result, libraryId, bookId);

            if (request.IsAdded)
            {
                return Created(renderResult.Links.Self(), renderResult);
            }

            return Ok(renderResult);
        }

        [HttpPut("library/{libraryId}/books/{bookId}/pages/{pageNumber}", Name = nameof(BookPageController.UpdatePage))]
        [Authorize]
        public async Task<IActionResult> UpdatePage(int libraryId, int bookId, int pageNumber, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpDelete("library/{libraryId}/books/{bookId}/pages/{pageNumber}", Name = nameof(BookPageController.DeletePage))]
        [Authorize]
        public async Task<IActionResult> DeletePage(int libraryId, int bookId, int pageNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpGet("library/{libraryId}/books/{bookId}/pages/{pageNumber}/image", Name = nameof(BookPageController.GetPageImage))]
        public async Task<IActionResult> GetPageImage(int libraryId, int bookId, int pageNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpPut("library/{libraryId}/books/{bookId}/pages/{pageNumber}/image")]
        public async Task<IActionResult> UpdatePageImage(int libraryId, int bookId, int pageNumber, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var request = new UpdateBookPageImageRequest(_userHelper.Claims, libraryId, bookId, pageNumber)
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
                var imageLink = _bookPageRenderer.RenderImageLink(libraryId, bookId, pageNumber);

                return new CreatedResult(imageLink.Href, null);
            }

            return new OkResult();
        }

        [HttpDelete("library/{libraryId}/books/{bookId}/pages/{pageNumber}/image")]
        [Authorize]
        public async Task<IActionResult> DeletePageImage(int libraryId, int bookId, int pageNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpPut("library/{libraryId}/books/{bookId}/pages/{pageNumber}/assign")]
        [Authorize]
        public async Task<IActionResult> AssignPage(int libraryId, int bookId, int pageNumber, int? userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpPut("library/{libraryId}/books/{bookId}/pages/{pageNumber}/status/{pageStatus}")]
        [Authorize]
        public async Task<IActionResult> SetPageStatus(int libraryId, int bookId, int pageNumber, int pageStatus, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
