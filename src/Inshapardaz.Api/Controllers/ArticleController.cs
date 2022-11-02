using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    public class ArticleController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderArticle _articleRenderer;
        private readonly IUserHelper _userHelper;

        public ArticleController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderArticle articleRenderer,
            IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _articleRenderer = articleRenderer;
            _userHelper = userHelper;
        }

        [HttpGet("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles", Name = nameof(ArticleController.GetArticlesByIssue))]
        public async Task<IActionResult> GetArticlesByIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken token = default(CancellationToken))
        {
            var query = new GetArticlesByIssueQuery(libraryId, periodicalId, volumeNumber, issueNumber);
            var articles = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (articles != null)
            {
                return new OkObjectResult(_articleRenderer.Render(articles, libraryId, periodicalId, volumeNumber, issueNumber));
            }

            return new NotFoundResult();
        }

        [HttpGet("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}", Name = nameof(ArticleController.GetArticleById))]
        public async Task<IActionResult> GetArticleById(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken token = default(CancellationToken))
        {
            var query = new GetArticleByIdQuery(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber);
            var article = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (article != null)
            {
                return new OkObjectResult(_articleRenderer.Render(article, libraryId, periodicalId, volumeNumber, issueNumber));
            }

            return new NotFoundResult();
        }

        [HttpPost("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles", Name = nameof(ArticleController.CreateArticle))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> CreateArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, [FromBody] ArticleView article, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AddArticleRequest(libraryId, periodicalId, volumeNumber, issueNumber, article.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                var renderResult = _articleRenderer.Render(request.Result, libraryId, periodicalId, volumeNumber, issueNumber);
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new BadRequestResult();
        }

        [HttpPut("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}", Name = nameof(ArticleController.UpdateArticle))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, [FromBody] ArticleView article, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new UpdateArticleRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, article.Map());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _articleRenderer.Render(request.Result.Article, libraryId, periodicalId, volumeNumber, issueNumber);

            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new OkObjectResult(renderResult);
        }

        [HttpDelete("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}", Name = nameof(ArticleController.DeleteArticle))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeleteArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteArticleRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPost("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/sequence", Name = nameof(ArticleController.UpdateArticleSequence))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateArticleSequence(int libraryId, int periodicalId, int volumeNumber, int issueNumber, [FromBody] IEnumerable<ArticleSequenceView> articles, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new UpdateIssueArticleSequenceRequest(libraryId, periodicalId, volumeNumber, issueNumber, articles.Select(a => a.Map()));
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            return new OkObjectResult(_articleRenderer.Render(request.Result, libraryId, periodicalId, volumeNumber, issueNumber));
        }

        [HttpGet("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}/contents", Name = nameof(ArticleController.GetArticleContent))]
        [Authorize()]
        public async Task<IActionResult> GetArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string langauge, CancellationToken token = default(CancellationToken))
        {
            //var language = Request.Headers["Accept-Language"]; // default to  ""

            var query = new GetArticleContentQuery(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, langauge);

            var chapterContents = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (chapterContents != null)
            {
                return new OkObjectResult(_articleRenderer.Render(chapterContents, libraryId));
            }

            return new NotFoundResult();
        }

        [HttpPost("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}/contents", Name = nameof(ArticleController.CreateArticleContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> CreateArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, [FromBody] string content, CancellationToken token = default(CancellationToken))
        {
            //var language = Request.Headers["Content-Language"];
            var request = new AddArticleContentRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, content, language);
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                var renderResult = _articleRenderer.Render(request.Result, libraryId);
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }


            return new BadRequestResult();
        }

        [HttpPut("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}/contents", Name = nameof(ArticleController.UpdateArticleContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, [FromBody] string content, CancellationToken token = default(CancellationToken))
        {
            //var language = Request.Headers["Content-Language"];

            var request = new UpdateArticleContentRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, content, language);
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _articleRenderer.Render(request.Result.Content, libraryId);
            if (request.Result != null && request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return Ok(renderResult);
        }

        [HttpDelete("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}/contents", Name = nameof(ArticleController.DeleteArticleContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeleteArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, CancellationToken token = default(CancellationToken))
        {
            //var language = Request.Headers["Accept-Language"];

            var request = new DeleteArticleContentRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, language);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPost("libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{sequenceNumber}/assign", Name = nameof(ArticleController.AssignArticleToUser))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        [Produces(typeof(ArticleView))]
        public async Task<IActionResult> AssignArticleToUser(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, [FromBody] ChapterAssignmentView assignment, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AssignArticleToUserRequest(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, assignment.AccountId ?? _userHelper.Account.Id, _userHelper.IsAdmin);

            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _articleRenderer.Render(request.Result, libraryId, periodicalId, volumeNumber, issueNumber);

            return Ok(renderResult);
        }

    }
}
