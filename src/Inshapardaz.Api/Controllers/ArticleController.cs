using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}/articles", Name = nameof(ArticleController.GetArticlesByIssue))]
        public async Task<IActionResult> GetArticlesByIssue(int libraryId, int periodicalId, int issueId, CancellationToken token = default(CancellationToken))
        {
            var query = new GetArticlesByIssueQuery(libraryId, periodicalId, issueId, _userHelper.GetUserId());
            var articles = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (articles != null)
            {
                return new OkObjectResult(_articleRenderer.Render(articles, libraryId, periodicalId, issueId));
            }

            return new NotFoundResult();
        }

        [HttpGet("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}/articles/{articleId}", Name = nameof(ArticleController.GetArticleById))]
        public async Task<IActionResult> GetArticleById(int libraryId, int periodicalId, int issueId, int articleId, int chapterId, CancellationToken token = default(CancellationToken))
        {
            var query = new GetArticleByIdQuery(libraryId, periodicalId, issueId, articleId, _userHelper.GetUserId());
            var article = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (article != null)
            {
                return new OkObjectResult(_articleRenderer.Render(article, libraryId, periodicalId, issueId));
            }

            return new NotFoundResult();
        }

        [HttpPost("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}/articles", Name = nameof(ArticleController.CreateArticle))]
        [Authorize]
        public async Task<IActionResult> CreateArticle(int libraryId, int periodicalId, int issueId, ArticleView article, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AddArticleRequest(_userHelper.Claims, libraryId, periodicalId, issueId, article.Map(), _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                var renderResult = _articleRenderer.Render(request.Result, libraryId, periodicalId, issueId);
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new BadRequestResult();
        }

        [HttpPut("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}/articles/{articleId}", Name = nameof(ArticleController.UpdateArticle))]
        [Authorize]
        public async Task<IActionResult> UpdateArticle(int libraryId, int periodicalId, int issueId, int articleId, ArticleView chapter, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new UpdateArticleRequest(_userHelper.Claims, libraryId, periodicalId, issueId, articleId, chapter.Map(), _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _articleRenderer.Render(request.Result.Article, libraryId, periodicalId, issueId);

            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new OkObjectResult(renderResult);
        }

        [HttpDelete("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}/articles/{articleId}", Name = nameof(ArticleController.DeleteArticle))]
        [Authorize]
        public async Task<IActionResult> DeleteArticle(int libraryId, int periodicalId, int issueId, int articleId, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteArticleRequest(_userHelper.Claims, libraryId, periodicalId, issueId, articleId);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpGet("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}/articles/{articleId}/contents", Name = nameof(ArticleController.GetArticleContent))]
        public async Task<IActionResult> GetArticleContent(int libraryId, int periodicalId, int issueId, int articleId, CancellationToken token = default(CancellationToken))
        {
            var contentType = Request.Headers["Accept"]; // default to "text/markdown"
            var language = Request.Headers["Accept-Language"]; // default to  ""

            var query = new GetArticleContentQuery(libraryId, periodicalId, issueId, articleId, language, contentType, _userHelper.GetUserId());

            var chapterContents = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (chapterContents != null)
            {
                return new OkObjectResult(_articleRenderer.Render(chapterContents, libraryId));
            }

            return new NotFoundResult();
        }

        [HttpPost("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}/articles/{articleId}/content", Name = nameof(ArticleController.CreateArticleContent))]
        [Authorize]
        public async Task<IActionResult> CreateArticleContent(int libraryId, int periodicalId, int issueId, int articleId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }
            var language = Request.Headers["Accept-Language"];

            var request = new AddArticleContentRequest(_userHelper.Claims, libraryId, periodicalId, issueId, articleId, Encoding.Default.GetString(content), language, file.ContentType, _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                var renderResult = _articleRenderer.Render(request.Result, libraryId);
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new BadRequestResult();
        }

        [HttpPut("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}/articles/{articleId}/content", Name = nameof(ArticleController.UpdateArticleContent))]
        [Authorize]
        public async Task<IActionResult> UpdateArticleContent(int libraryId, int periodicalId, int issueId, int articleId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var language = Request.Headers["Accept-Language"];

            var request = new UpdateArticleContentRequest(_userHelper.Claims, libraryId, periodicalId, issueId, articleId, Encoding.Default.GetString(content), language, file.ContentType, _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                var renderResult = _articleRenderer.Render(request.Result.Content, libraryId);
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new BadRequestResult();
        }

        [HttpDelete("library/{libraryId}/periodicals/{periodicalId}/issues/{issueId}/articles/{articleId}/content", Name = nameof(ArticleController.DeleteArticleContent))]
        [Authorize]
        public async Task<IActionResult> DeleteArticleContent(int libraryId, int periodicalId, int issueId, int articleId, CancellationToken token = default(CancellationToken))
        {
            var contentType = Request.Headers["Content-Type"];
            var language = Request.Headers["Accept-Language"];

            var request = new DeleteArticleContentRequest(_userHelper.Claims, libraryId, periodicalId, issueId, articleId, language, contentType, _userHelper.GetUserId());
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }
    }
}
