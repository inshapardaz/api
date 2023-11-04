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
using Inshapardaz.Domain.Ports.Handlers.Library.Article;
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
        private readonly IRenderFile _fileRenderer;

        public ArticleController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderArticle articleRenderer,
            IUserHelper userHelper,
            IRenderFile fileRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _articleRenderer = articleRenderer;
            _userHelper = userHelper;
            _fileRenderer = fileRenderer;

        }

        [HttpGet("libraries/{libraryId}/articles", Name = nameof(ArticleController.GetArticles))]
        public async Task<IActionResult> GetArticles(int libraryId, string query,
            int pageNumber = 1,
            int pageSize = 10,
            [FromQuery] int? authorId = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] bool? favorite = null,
            [FromQuery] bool? read = null,
            [FromQuery] EditingStatus status = EditingStatus.All,
            [FromQuery] ArticleType type = ArticleType.Unknown,
            [FromQuery] AssignmentStatus assignedFor = AssignmentStatus.None,
            [FromQuery] ArticleSortByType sortBy = ArticleSortByType.Title,
            [FromQuery] SortDirection sortDirection = SortDirection.Ascending,
            CancellationToken token = default(CancellationToken))
        {
            var filter = new ArticleFilter
            {
                AuthorId = authorId,
                CategoryId = categoryId,
                Favorite = favorite,
                Read = read,
                Status = status,
                Type = type,
                AssignmentStatus = assignedFor
            };
            var articlesQuery = new GetArticlesQuery(libraryId, pageNumber, pageSize, _userHelper.Account?.Id)
            {
                Query = query,
                Filter = filter,
                SortBy = sortBy,
                SortDirection = sortDirection
            };
            var articles = await _queryProcessor.ExecuteAsync(articlesQuery, cancellationToken: token);

            var args = new PageRendererArgs<ArticleModel, ArticleFilter, ArticleSortByType>
            {
                Page = articles,
                RouteArguments = new PagedRouteArgs<ArticleSortByType>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Query = query,
                    SortBy = sortBy,
                    SortDirection = sortDirection
                },
                Filters = filter,
            };

            return new OkObjectResult(_articleRenderer.Render(args, libraryId));
        }

        [HttpGet("libraries/{libraryId}/articles/{articleId}", Name = nameof(ArticleController.GetArticle))]
        public async Task<IActionResult> GetArticle(int libraryId, int articleId, CancellationToken token = default(CancellationToken))
        {
            var query = new GetArticleByIdQuery(libraryId, articleId);
            var article = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (article != null)
            {
                return new OkObjectResult(_articleRenderer.Render(article, libraryId));
            }

            return new NotFoundResult();
        }

        [HttpPost("libraries/{libraryId}/articles", Name = nameof(ArticleController.CreateArticle))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> CreateArticle(int libraryId, [FromBody] ArticleView article, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AddArticleRequest(libraryId, article.Map())
            {
                AccountId = _userHelper.Account.Id
            };

            await _commandProcessor.SendAsync(request, cancellationToken: token);

            if (request.Result != null)
            {
                var renderResult = _articleRenderer.Render(request.Result, libraryId);
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new BadRequestResult();
        }

        [HttpPut("libraries/{libraryId}/articles/{articleId}", Name = nameof(ArticleController.UpdateArticle))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateArticle(int libraryId, int articleId, [FromBody] ArticleView article, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new UpdateArticleRequest(libraryId, articleId, article.Map())
            {
                AccountId = _userHelper.Account.Id
            };
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _articleRenderer.Render(request.Result.Article, libraryId);

            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new OkObjectResult(renderResult);
        }

        [HttpDelete("libraries/{libraryId}/articles/{articleId}", Name = nameof(ArticleController.DeleteArticle))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeleteArticle(int libraryId, int articleId, CancellationToken token = default(CancellationToken))
        {
            var request = new DeleteArticleRequest(libraryId, articleId);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPut("libraries/{libraryId}/articles/{articleId}/image", Name = nameof(ArticleController.UpdateArticleImage))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateArticleImage(int libraryId, long articleId, IFormFile file, CancellationToken token = default(CancellationToken))
        {
            var content = new byte[file.Length];
            using (var stream = new MemoryStream(content))
            {
                await file.CopyToAsync(stream);
            }

            var request = new UpdateArticleImageRequest(libraryId, articleId, _userHelper.Account?.Id)
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
                var response = _fileRenderer.Render(libraryId, request.Result.File);

                return new CreatedResult(response.Links.Self(), response);
            }

            return new OkResult();
        }

        [HttpGet("libraries/{libraryId}/articles/{articleId}/contents", Name = nameof(ArticleController.GetArticleContent))]
        [Authorize()]
        public async Task<IActionResult> GetArticleContent(int libraryId, int articleId, string language, CancellationToken token = default(CancellationToken))
        {
            var parsedLanguage = Request.Headers["Accept-Language"]; // default to  ""

            var query = new GetArticleContentQuery(libraryId, articleId, language ?? parsedLanguage);

            var articleContents = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

            if (articleContents != null)
            {
                return new OkObjectResult(_articleRenderer.Render(articleContents, libraryId, articleId));
            }

            return new NotFoundResult();
        }

        [HttpPut("libraries/{libraryId}/articles/{articleId}/contents", Name = nameof(ArticleController.UpdateArticleContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> UpdateArticleContent(int libraryId, int articleId, string language, [FromBody] string content, CancellationToken token = default(CancellationToken))
        {
            var parsedLanguage = Request.Headers["Content-Language"];

            var request = new UpdateArticleContentRequest(libraryId, articleId, content, language ?? parsedLanguage);
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _articleRenderer.Render(request.Result.Content, libraryId, articleId);
            if (request.Result != null && request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return Ok(renderResult);
        }

        [HttpDelete("libraries/{libraryId}/articles/{articleId}/contents", Name = nameof(ArticleController.DeleteArticleContent))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        public async Task<IActionResult> DeleteArticleContent(int libraryId, int articleId, string language, CancellationToken token = default(CancellationToken))
        {
            var parsedLanguage = Request.Headers["Accept-Language"];

            var request = new DeleteArticleContentRequest(libraryId, articleId, language ?? parsedLanguage);
            await _commandProcessor.SendAsync(request, cancellationToken: token);
            return new NoContentResult();
        }

        [HttpPost("libraries/{libraryId}/articles/{articleId}/assign", Name = nameof(ArticleController.AssignArticleToUser))]
        [Authorize(Role.Admin, Role.LibraryAdmin, Role.Writer)]
        [Produces(typeof(IssueArticleView))]
        public async Task<IActionResult> AssignArticleToUser(int libraryId, int articleId, [FromBody] AssignmentView assignment, CancellationToken token = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var request = new AssignArticleToUserRequest(libraryId, articleId, assignment.AccountId ?? _userHelper.Account.Id, _userHelper.IsAdmin);

            await _commandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = _articleRenderer.Render(request.Result, libraryId);

            return Ok(renderResult);
        }

        [HttpPost("libraries/{libraryId}/favorites/articles/{articleId}", Name = nameof(ArticleController.AddArticleToFavorites))]
        [Authorize]
        public async Task<IActionResult> AddArticleToFavorites(int libraryId, int articleId, CancellationToken token)
        {
            var request = new AddArticleToFavoriteRequest(libraryId, articleId, _userHelper.Account?.Id);
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            return new OkResult();
        }

        [HttpDelete("libraries/{libraryId}/favorites/articles/{articleId}", Name = nameof(ArticleController.RemoveArtiucleFromFavorites))]
        [Authorize]
        public async Task<IActionResult> RemoveArtiucleFromFavorites(int libraryId, int articleId, CancellationToken token)
        {
            var request = new RemoveArticleFromFavoriteRequest(libraryId, articleId, _userHelper.Account?.Id);
            await _commandProcessor.SendAsync(request, cancellationToken: token);

            return new OkResult();
        }
    }
}
