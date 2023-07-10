using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Handlers.Library.Periodical.Issue;
using Inshapardaz.Domain.Ports.Handlers.Library.Periodical.Issue.Article;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers
{
    public class IssueArticleController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderIssueArticle _issueArticleRenderer;
        private readonly IUserHelper _userHelper;

        public IssueArticleController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderIssueArticle articleRenderer,
            IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _issueArticleRenderer = articleRenderer;
            _userHelper = userHelper;
        }

        [HttpGet("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles", Name = nameof(IssueArticleController.GetIssueArticles))]
        public async Task<IActionResult> GetIssueArticles(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken token = default(CancellationToken))
        {
            var query = new GetIssueArticlesQuery(libraryId, periodicalId, volumeNumber, issueNumber);
            var articles = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (articles != null)
            {
                return new OkObjectResult(_issueArticleRenderer.Render(articles, libraryId, periodicalId, volumeNumber, issueNumber));
            }

            return new NotFoundResult();
        }

        [HttpGet("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}", Name = nameof(IssueArticleController.GetIssueArticleById))]
        public async Task<IActionResult> GetIssueArticleById(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken token = default(CancellationToken))
        {
            var query = new GetIssueArticleByIdQuery(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber);
            var article = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (article != null)
            {
                return new OkObjectResult(_issueArticleRenderer.Render(article, libraryId, periodicalId, volumeNumber, issueNumber));
            }

            return new NotFoundResult();
        }

        [HttpPost("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles", Name = nameof(IssueArticleController.CreateIssueArticle))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> CreateIssueArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, [FromBody] IssueArticleView article, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AddIssueArticleRequest(libraryId, periodicalId, volumeNumber, issueNumber, article.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                var renderResult = _issueArticleRenderer.Render(request.Result, libraryId, periodicalId, volumeNumber, issueNumber);
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new BadRequestResult();
        }

        [HttpPut("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}", Name = nameof(IssueArticleController.UpdateIssueArticle))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateIssueArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, [FromBody] IssueArticleView article, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new UpdateIssueArticleRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, article.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _issueArticleRenderer.Render(request.Result.Article, libraryId, periodicalId, volumeNumber, issueNumber);

            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new OkObjectResult(renderResult);
        }

        [HttpDelete("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}", Name = nameof(IssueArticleController.DeleteIssueArticle))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeleteIssueArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteIssueArticleRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPost("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/sequence", Name = nameof(IssueArticleController.UpdateIssueArticleSequence))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateIssueArticleSequence(int libraryId, int periodicalId, int volumeNumber, int issueNumber, [FromBody] IEnumerable<ArticleSequenceView> articles, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new UpdateIssueArticleSequenceRequest(libraryId, periodicalId, volumeNumber, issueNumber, articles.Select(a => a.Map()));
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            return new OkObjectResult(_issueArticleRenderer.Render(request.Result, libraryId, periodicalId, volumeNumber, issueNumber));
        }

        [HttpGet("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}/contents", Name = nameof(IssueArticleController.GetIssueArticleContent))]
        [Authorize()]
        public async Task<IActionResult> GetIssueArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string langauge, CancellationToken token = default(CancellationToken))
        {
            var parsedLanguage = Request.Headers["Accept-Language"]; // default to  ""

            var query = new GetIssueArticleContentQuery(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, langauge ?? parsedLanguage);

            var chapterContents = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (chapterContents != null)
            {
                return new OkObjectResult(_issueArticleRenderer.Render(chapterContents, libraryId));
            }

            return new NotFoundResult();
        }

        [HttpPost("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}/contents", Name = nameof(IssueArticleController.CreateIssueArticleContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> CreateIssueArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, [FromBody] string content, CancellationToken token = default(CancellationToken))
        {
            var parsedLanguage = Request.Headers["Content-Language"];
            var request = new AddIssueArticleContentRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, content, language ?? parsedLanguage);
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                var renderResult = _issueArticleRenderer.Render(request.Result, libraryId);
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }


            return new BadRequestResult();
        }

        [HttpPut("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}/contents", Name = nameof(IssueArticleController.UpdateIssueArticleContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateIssueArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, [FromBody] string content, CancellationToken token = default(CancellationToken))
        {
            var parsedLanguage = Request.Headers["Content-Language"];

            var request = new UpdateIssueArticleContentRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, content, language ?? parsedLanguage);
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _issueArticleRenderer.Render(request.Result.Content, libraryId);
            if (request.Result != null && request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return Ok(renderResult);
        }

        [HttpDelete("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}/contents", Name = nameof(IssueArticleController.DeleteIssueArticleContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeleteIssueArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, CancellationToken token = default(CancellationToken))
        {
            var parsedLanguage = Request.Headers["Accept-Language"];

            var request = new DeleteIssueArticleContentRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, language ?? parsedLanguage);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPost("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}/assign", Name = nameof(IssueArticleController.AssignIssueArticleToUser))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        [Produces(typeof(IssueArticleView))]
        public async Task<IActionResult> AssignIssueArticleToUser(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, [FromBody] ChapterAssignmentView assignment, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AssignIssueArticleToUserRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, assignment.AccountId ?? _userHelper.Account.Id, _userHelper.IsAdmin);

            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _issueArticleRenderer.Render(request.Result, libraryId, periodicalId, volumeNumber, issueNumber);

            return Ok(renderResult);
        }

    }
}
