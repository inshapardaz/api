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
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Controllers.Library
{
    public class IssueController : Controller
    {
        private IAmACommandProcessor _commandProcessor;
        private readonly IRenderIssues _IssuesRenderer;
        private readonly IRenderIssue _IssueRenderer;
        private readonly IRenderFile _fileRenderer;

        public IssueController(IAmACommandProcessor commandProcessor, 
            IRenderIssues IssuesRenderer, 
            IRenderIssue IssueRenderer,
            IRenderFile fileRenderer)
        {
            _commandProcessor = commandProcessor;
            _IssuesRenderer = IssuesRenderer;
            _IssueRenderer = IssueRenderer;
            _fileRenderer = fileRenderer;
        }

        [HttpGet("/api/periodicals/{id}/issues", Name = "GetIssues")]
        [Produces(typeof(IEnumerable<IssueView>))]
        public async Task<IActionResult> GetList(int id, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new GetIssuesRequest(id, pageNumber, pageSize);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var args = new PageRendererArgs<Issue>
            {
                Page = request.Result,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize },
                RouteName = "GetIssues"
            };

            return Ok(_IssuesRenderer.Render(args));
        }

        [HttpGet("/api/periodicals/{id}/issues/{issueId}", Name = "GetIssueById")]
        [Produces(typeof(IssueView))]
        public async Task<IActionResult> Get(int id, int issueId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new GetIssueByIdRequest(id, issueId);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result != null)
                return Ok(_IssueRenderer.Render(request.Result));
            return NotFound();
        }

        [Authorize]
        [HttpPost("/api/periodicals/{id}/issues", Name = "CreateIssue")]
        [Produces(typeof(IssueView))]
        public async Task<IActionResult> Post(int id, [FromBody]IssueView value, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new AddIssueRequest(id, value.Map<IssueView, Issue>());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            var renderResult = _IssueRenderer.Render(request.Result);
            return Created(renderResult.Links.Self(), renderResult);
        }

        [Authorize]
        [HttpPut("/api/periodicals/{id}/issues/{issueId}", Name = "UpdateIssue")]
        [Produces(typeof(IssueView))]
        public async Task<IActionResult> Put(int id, int issueId, [FromBody]IssueView value, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new UpdateIssueRequest(id, issueId, value.Map<IssueView, Issue>());
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

            if (request.Result.HasAddedNew)
            {
                var renderResult = _IssueRenderer.Render(request.Result.Issue);
                return Created(renderResult.Links.Self(), renderResult);
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("/api/periodicals/{id}/issues/{issueId}", Name = "DeleteIssue")]
        public async Task<IActionResult> Delete(int id, int issueId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new DeleteIssueRequest(id, issueId);
            await _commandProcessor.SendAsync(request, cancellationToken: cancellationToken);
            return NoContent();
        }

        [Authorize]
        [HttpPost("/api/periodicals/{id}/issues/{issueId}/image", Name = "UpdateIssueImage")]
        [ValidateModel]
        public async Task<IActionResult> PutImage(int id, int issueId, IFormFile file, CancellationToken cancellationToken)
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var request = new UpdateIssueImageRequest(id, issueId)
            {
                Image = new Domain.Entities.File
                {
                    FileName = file.FileName,
                    MimeType = file.ContentType,
                    Contents = content
                }
            };

            await _commandProcessor.SendAsync(request);


            if (request.Result.HasAddedNew)
            {
                var response = _fileRenderer.Render(request.Result.File);
                return Created(response.Links.Self(), response);
            }

            return NoContent();
        }
    }
}
