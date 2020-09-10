using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers
{
    public class IssueController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderIssue _issueRenderer;
        private readonly IRenderFile _fileRenderer;
        private readonly IUserHelper _userHelper;

        public IssueController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderIssue issueRenderer,
            IRenderFile fileRenderer,
            IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _issueRenderer = issueRenderer;
            _fileRenderer = fileRenderer;
            _userHelper = userHelper;
        }

        [HttpGet("library/{libraryId}/periodicals/{periodicalId}/issues", Name = nameof(IssueController.GetIssues))]
        public async Task<IActionResult> GetIssues(int libraryId, int periodicalId, string query, int pageNumber = 1, int pageSize = 10, CancellationToken token = default(CancellationToken))
        {
            var issuesQuery = new GetIssuesQuery(libraryId, periodicalId, pageNumber, pageSize) { Query = query };
            var result = await _queryProcessor.ExecuteAsync(issuesQuery, token);

            var args = new PageRendererArgs<IssueModel>
            {
                Page = result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
            };

            return new OkObjectResult(_issueRenderer.Render(args, libraryId));
        }

        [HttpGet("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}", Name = nameof(IssueController.GetIssueById))]
        public async Task<IActionResult> GetIssueById(int libraryId, int periodicalId, int issueId, CancellationToken token = default(CancellationToken))
        {
            var query = new GetIssueByIdQuery(libraryId, periodicalId, issueId);
            var issues = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (issues != null)
            {
                return new OkObjectResult(_issueRenderer.Render(issues, libraryId));
            }

            return new NotFoundResult();
        }

        [HttpPost("library/{libraryId}/periodicals{periodicalId}/issues", Name = nameof(IssueController.CreateIssue))]
        [Authorize(Roles = "Admin, Writer")]
        public async Task<IActionResult> CreateIssue(int libraryId, int periodicalId, IssueView issue, CancellationToken token = default(CancellationToken))
        {
            var request = new AddIssueRequest(_userHelper.Claims, libraryId, periodicalId, issue.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _issueRenderer.Render(request.Result, libraryId);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        [HttpPut("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}", Name = nameof(IssueController.UpdateIssue))]
        [Authorize(Roles = "Admin, Writer")]
        public async Task<IActionResult> UpdateIssue(int libraryId, int periodicalId, int issueId, IssueView issue, CancellationToken token = default(CancellationToken))
        {
            var request = new UpdateIssueRequest(_userHelper.Claims, libraryId, periodicalId, issueId, issue.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _issueRenderer.Render(request.Result.Issue, libraryId);
            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }
            else
            {
                return new OkObjectResult(renderResult);
            }
        }

        [HttpDelete("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}", Name = nameof(IssueController.DeleteIssue))]
        [Authorize(Roles = "Admin, Writer")]
        public async Task<IActionResult> DeleteIssue(int libraryId, int periodicalId, int issueId, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteIssueRequest(_userHelper.Claims, libraryId, periodicalId, issueId);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPut("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}/image", Name = nameof(IssueController.UpdateIssueImage))]
        [Authorize(Roles = "Admin, Writer")]
        public async Task<IActionResult> UpdateIssueImage(int libraryId, int periodicalId, int issueId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var request = new UpdateIssueImageRequest(_userHelper.Claims, libraryId, periodicalId, issueId)
            {
                Image = new Domain.Models.FileModel
                {
                    FileName = file.FileName,
                    MimeType = file.ContentType,
                    Contents = content
                }
            };

            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result.HasAddedNew)
            {
                var response = _fileRenderer.Render(request.Result.File);
                return new CreatedResult(response.Links.Self(), response);
            }

            return new OkResult();
        }

        [HttpPost("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}/content", Name = nameof(IssueController.CreateIssueContent))]
        [Authorize]
        public async Task<IActionResult> CreateIssueContent(int libraryId, int periodicalId, int issueId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }
            var language = Request.Headers["Accept-Language"];
            var mimeType = file.ContentType;

            var request = new AddIssueContentRequest(_userHelper.Claims, libraryId, periodicalId, issueId, language, mimeType)
            {
                Content = new FileModel
                {
                    Contents = content,
                    MimeType = mimeType,
                    DateCreated = DateTime.Now,
                    FileName = file.FileName
                }
            };

            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                var response = _issueRenderer.Render(request.Result, libraryId);
                return new CreatedResult(response.Links.Self(), response);
            }

            return new BadRequestResult();
        }

        [HttpPut("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}/content", Name = nameof(IssueController.UpdateIssueContent))]
        [Authorize]
        public async Task<IActionResult> UpdateIssueContent(int libraryId, int periodicalId, int issueId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }
            var language = Request.Headers["Accept-Language"];
            var mimeType = file.ContentType;

            var request = new UpdateIssueContentRequest(_userHelper.Claims, libraryId, periodicalId, issueId, language, mimeType)
            {
                Content = new FileModel
                {
                    Contents = content,
                    MimeType = mimeType,
                    DateCreated = DateTime.Now,
                    FileName = file.FileName
                }
            };

            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result.Content != null)
            {
                var renderResult = _issueRenderer.Render(request.Result.Content, libraryId);

                if (request.Result.HasAddedNew)
                {
                    return new CreatedResult(renderResult.Links.Self(), renderResult);
                }
                else
                {
                    return new OkObjectResult(renderResult);
                }
            }

            return new BadRequestResult();
        }

        [HttpDelete("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}/content", Name = nameof(IssueController.DeleteIssueContent))]
        [Authorize]
        public async Task<IActionResult> DeleteIssueContent(int libraryId, int periodicalId, int issueId, CancellationToken token = default(CancellationToken))
        {
            var mimeType = Request.Headers["Content-Type"];
            var language = Request.Headers["Accept-Language"];

            var request = new DeleteIssueContentRequest(_userHelper.Claims, libraryId, periodicalId, issueId, language, mimeType, _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }
    }
}
