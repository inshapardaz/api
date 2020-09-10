using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers
{
    public class PageController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderPage _pageRenderer;
        private readonly IUserHelper _userHelper;

        public PageController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderPage pageRenderer,
            IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _pageRenderer = pageRenderer;
            _userHelper = userHelper;
        }

        [HttpGet("library/{libraryId}/books/{bookId}/pages")]
        public async Task<IActionResult> GetPagesByBook(int libraryId, int bookId, int pageNumber = 1,
            int pageSize = 10, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpGet("library/{libraryId}/books/{bookId}/pages/{pageIndex}")]
        public async Task<IActionResult> GetPagesByIndex(int libraryId, int bookId, int pageIndex, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpPost("library/{libraryId}/books/{bookId}/pages")]
        [Authorize]
        public async Task<IActionResult> CreatePage(int libraryId, int bookId, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpPut("library/{libraryId}/books/{bookId}/pages/{pageIndex}")]
        [Authorize]
        public async Task<IActionResult> UpdatePage(int libraryId, int bookId, int pageIndex, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpDelete("library/{libraryId}/books/{bookId}/pages/{pageIndex}")]
        [Authorize]
        public async Task<IActionResult> DeletePage(int libraryId, int bookId, int pageIndex, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpGet("library/{libraryId}/books/{bookId}/pages/{pageIndex}/content")]
        public async Task<IActionResult> GetPageContent(int libraryId, int bookId, int pageIndex, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpPut("library/{libraryId}/books/{bookId}/pages/{pageIndex}/content")]
        [Authorize]
        public async Task<IActionResult> UpdatePageContent(int libraryId, int bookId, int pageIndex, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpDelete("library/{libraryId}/books/{bookId}/pages/{pageIndex}/content")]
        [Authorize]
        public async Task<IActionResult> DeletePageContent(int libraryId, int bookId, int pageIndex, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpPut("library/{libraryId}/books/{bookId}/pages/{pageIndex}/assign")]
        [Authorize]
        public async Task<IActionResult> AssignPage(int libraryId, int bookId, int pageIndex, int? userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        [HttpPut("library/{libraryId}/books/{bookId}/pages/{pageIndex}/status/{pageStatus}")]
        [Authorize]
        public async Task<IActionResult> SetPageStatus(int libraryId, int bookId, int pageIndex, int pageStatus, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
