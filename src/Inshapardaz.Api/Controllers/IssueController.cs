using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

namespace Inshapardaz.Api.Controllers
{
    public class IssueController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderIssue _issueRenderer;
        private readonly IRenderFile _fileRenderer;

        public IssueController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderIssue issueRenderer,
            IRenderFile fileRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _issueRenderer = issueRenderer;
            _fileRenderer = fileRenderer;
        }

        [HttpGet("libraries/{libraryId}/periodicals/{periodicalId}/issues", Name = nameof(IssueController.GetIssues))]
        public async Task<IActionResult> GetIssues(int libraryId, int periodicalId, string query, int pageNumber = 1, int pageSize = 10, CancellationToken token = default(CancellationToken))
        {
            var issuesQuery = new GetIssuesQuery(libraryId, periodicalId, pageNumber, pageSize) { Query = query };
            var result = await _queryProcessor.ExecuteAsync(issuesQuery, token);

            var args = new PageRendererArgs<IssueModel>
            {
                Page = result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
            };

            return new OkObjectResult(_issueRenderer.Render(args, libraryId, periodicalId));
        }

        [HttpGet("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}", Name = nameof(IssueController.GetIssueById))]
        public async Task<IActionResult> GetIssueById(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken token = default(CancellationToken))
        {
            var query = new GetIssueByIdQuery(libraryId, periodicalId, volumeNumber, issueNumber);
            var issues = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (issues != null)
            {
                return new OkObjectResult(_issueRenderer.Render(issues, libraryId));
            }

            return new NotFoundResult();
        }


        [HttpPost("libraries/{libraryId}/periodicals{periodicalId}/issues", Name = nameof(IssueController.CreateIssue))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> CreateIssue(int libraryId, int periodicalId, [FromBody]IssueView issue, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AddIssueRequest(libraryId, periodicalId, issue.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _issueRenderer.Render(request.Result, libraryId);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        [HttpPut("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}", Name = nameof(IssueController.UpdateIssue))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, [FromBody] IssueView issue, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new UpdateIssueRequest(libraryId, periodicalId, volumeNumber, issueNumber, issue.Map());
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

        [HttpDelete("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}", Name = nameof(IssueController.DeleteIssue))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeleteIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteIssueRequest(libraryId, periodicalId, volumeNumber, issueNumber);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPut("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/image", Name = nameof(IssueController.UpdateIssueImage))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateIssueImage(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var request = new UpdateIssueImageRequest(libraryId, periodicalId, volumeNumber, issueNumber)
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

        [HttpPost("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/content", Name = nameof(IssueController.CreateIssueContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> CreateIssueContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }
            var language = Request.Headers["Accept-Language"];
            var mimeType = file.ContentType;

            var request = new AddIssueContentRequest(libraryId, periodicalId, volumeNumber, issueNumber, language, mimeType)
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

        [HttpPut("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/content", Name = nameof(IssueController.UpdateIssueContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateIssueContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }
            var language = Request.Headers["Accept-Language"];
            var mimeType = file.ContentType;

            var request = new UpdateIssueContentRequest(libraryId, periodicalId, volumeNumber, issueNumber, language, mimeType)
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

        [HttpDelete("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/content", Name = nameof(IssueController.DeleteIssueContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeleteIssueContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken token = default(CancellationToken))
        {
            var mimeType = Request.Headers["Content-Type"];
            var language = Request.Headers["Accept-Language"];

            var request = new DeleteIssueContentRequest(libraryId, periodicalId, volumeNumber, issueNumber, language, mimeType);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }
    }
}
