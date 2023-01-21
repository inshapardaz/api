﻿using Inshapardaz.Api.Converters;
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
    public class UserController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderBook _bookRenderer;
        private readonly IRenderBookPage _bookPageRenderer;
        private readonly IRenderIssuePage _issuePageRenderer;
        private readonly IRenderFile _fileRenderer;
        private readonly IUserHelper _userHelper;

        public UserController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderBook bookRenderer,
            IRenderBookPage bookPageRenderer,
            IRenderIssuePage issuePageRenderer,
            IRenderFile fileRenderer,
            IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _bookRenderer = bookRenderer;
            _bookPageRenderer = bookPageRenderer;
            _issuePageRenderer = issuePageRenderer;
            _fileRenderer = fileRenderer;
            _userHelper = userHelper;
        }

        [HttpGet("libraries/{libraryId}/my/summary", Name = nameof(UserController.GetUserPublicationSummary))]
        [Produces(typeof(IEnumerable<UserPageSummaryView>))]
        public async Task<IActionResult> GetUserPublicationSummary(int libraryId, CancellationToken token = default(CancellationToken))
        {
            var accountId = _userHelper.Account.Id;
            var getBookPagesQuery = new GetUserPublicationSummary(libraryId, accountId);
            var result = await _queryProcessor.ExecuteAsync(getBookPagesQuery, token);

            return new OkObjectResult(result.Select(x => x.Map()));
        }

        [HttpGet("libraries/{libraryId}/books/my/pages", Name = nameof(UserController.GetBookPagesByUser))]
        [Produces(typeof(PageView<BookPageView>))]
        public async Task<IActionResult> GetBookPagesByUser(int libraryId,
            int pageNumber = 1,
            int pageSize = 10,
            [FromQuery] EditingStatus status = EditingStatus.All,
            [FromQuery] AssignmentFilter assignmentFilter = AssignmentFilter.All,
            CancellationToken token = default(CancellationToken))
        {
            var accountId = _userHelper.Account.Id;
            var getBookPagesQuery = new GetBookPagesForUserQuery(libraryId, accountId, pageNumber, pageSize)
            {
                StatusFilter = status,
            };
            var result = await _queryProcessor.ExecuteAsync(getBookPagesQuery, token);

            var args = new PageRendererArgs<BookPageModel, PageFilter>
            {
                Page = result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                Filters = new PageFilter { Status = status }
            };

            return new OkObjectResult(_bookPageRenderer.RenderUserPages(args, libraryId));
        }


        [HttpGet("libraries/{libraryId}/periodicals/my/pages", Name = nameof(UserController.GetIssuePagesByUser))]
        [Produces(typeof(PageView<BookPageView>))]
        public async Task<IActionResult> GetIssuePagesByUser(int libraryId,
           int pageNumber = 1,
           int pageSize = 10,
           [FromQuery] EditingStatus status = EditingStatus.All,
           [FromQuery] AssignmentFilter assignmentFilter = AssignmentFilter.All,
           CancellationToken token = default(CancellationToken))
        {
            var accountId = _userHelper.Account.Id;
            var getBookPagesQuery = new GetIssuePagesForUserQuery(libraryId, accountId, pageNumber, pageSize)
            {
                StatusFilter = status,
            };
            var result = await _queryProcessor.ExecuteAsync(getBookPagesQuery, token);

            var args = new PageRendererArgs<IssuePageModel, PageFilter>
            {
                Page = result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                Filters = new PageFilter { Status = status }
            };

            return new OkObjectResult(_issuePageRenderer.RenderUserPages(args, libraryId));
        }

        [HttpGet("libraries/{libraryId}/my/books", Name = nameof(UserController.GetBooksByUser))]
        [Produces(typeof(PageView<BookView>))]
        public async Task<IActionResult> GetBooksByUser(int libraryId,
            int pageNumber = 1,
            int pageSize = 10,
            [FromQuery] BookStatuses status = BookStatuses.BeingTyped,
            CancellationToken token = default(CancellationToken))
        {
            var accountId = _userHelper.Account.Id;
            var getBookPagesQuery = new GetUserBooksQuery(libraryId, accountId, pageNumber, pageSize)
            {
                StatusFilter = status,
            };
            var result = await _queryProcessor.ExecuteAsync(getBookPagesQuery, token);

            var args = new PageRendererArgs<BookModel, BookFilter>
            {
                Page = result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                Filters = new BookFilter { Status = status }
            };

            return new OkObjectResult(_bookRenderer.Render(args, libraryId));
        }
    }
}