using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library.Article;
using Inshapardaz.Domain.Ports.Query.Library.Article;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

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
        var articlesQuery = new GetArticlesQuery(libraryId, pageNumber, pageSize, _userHelper.AccountId)
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
    public async Task<IActionResult> CreateArticle(int libraryId, [FromBody] ArticleView article, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new AddArticleRequest(libraryId, article.Map())
        {
            AccountId = _userHelper.AccountId
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
    public async Task<IActionResult> UpdateArticle(int libraryId, int articleId, [FromBody] ArticleView article, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var articleToUpdate = article.Map();
        articleToUpdate.Id = articleId;
        var request = new UpdateArticleRequest(libraryId, articleToUpdate)
        {
            AccountId = _userHelper.AccountId
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
    public async Task<IActionResult> DeleteArticle(int libraryId, int articleId, CancellationToken token = default(CancellationToken))
    {
        var request = new DeleteArticleRequest(libraryId, articleId);
        await _commandProcessor.SendAsync(request, cancellationToken: token);
        return new NoContentResult();
    }

    [HttpPut("libraries/{libraryId}/articles/{articleId}/image", Name = nameof(ArticleController.UpdateArticleImage))]
    public async Task<IActionResult> UpdateArticleImage(int libraryId, long articleId, IFormFile file, CancellationToken token = default(CancellationToken))
    {
        var content = new byte[file.Length];
        using (var stream = new MemoryStream(content))
        {
            await file.CopyToAsync(stream);
        }

        var request = new UpdateArticleImageRequest(libraryId, articleId, _userHelper.AccountId)
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
    public async Task<IActionResult> GetArticleContent(int libraryId, int articleId, [FromQuery] string language, CancellationToken token = default(CancellationToken))
    {
        var query = new GetArticleContentQuery(libraryId, articleId, language);

        var articleContents = await _queryProcessor.ExecuteAsync(query, cancellationToken: token);

        if (articleContents != null)
        {
            return new OkObjectResult(_articleRenderer.Render(articleContents, libraryId, articleId));
        }

        return new NotFoundResult();
    }

    [HttpPost("libraries/{libraryId}/articles/{articleId}/contents", Name = nameof(CreateArticleContent))]
    public async Task<IActionResult> CreateArticleContent(int libraryId, long articleId, [FromBody] ArticleContentView content, CancellationToken token = default(CancellationToken))
    {
        var contentPayload = content.Map();
        contentPayload.ArticleId = articleId;

        var request = new AddArticleContentRequest(libraryId)
        {
            Content = contentPayload
        };
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        if (request.Result != null)
        {
            var renderResult = _articleRenderer.Render(request.Result, libraryId, articleId);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        return new BadRequestResult();
    }

    [HttpPut("libraries/{libraryId}/articles/{articleId}/contents", Name = nameof(ArticleController.UpdateArticleContent))]
    public async Task<IActionResult> UpdateArticleContent(int libraryId, int articleId, [FromBody] ArticleContentView content, CancellationToken token = default(CancellationToken))
    {
        var contentPayload = content.Map();
        contentPayload.ArticleId = articleId;

        var request = new UpdateArticleContentRequest(libraryId)
        {
            Content = contentPayload
        };
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = _articleRenderer.Render(request.Result.Content, libraryId, articleId);
        if (request.Result != null && request.Result.HasAddedNew)
        {
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        return Ok(renderResult);
    }

    [HttpDelete("libraries/{libraryId}/articles/{articleId}/contents", Name = nameof(ArticleController.DeleteArticleContent))]
    public async Task<IActionResult> DeleteArticleContent(int libraryId, int articleId, [FromQuery] string language, CancellationToken token = default(CancellationToken))
    {
        var request = new DeleteArticleContentRequest(libraryId, articleId, language);
        await _commandProcessor.SendAsync(request, cancellationToken: token);
        return new NoContentResult();
    }

    [HttpPost("libraries/{libraryId}/articles/{articleId}/assign", Name = nameof(ArticleController.AssignArticleToUser))]
    [Produces(typeof(IssueArticleView))]
    public async Task<IActionResult> AssignArticleToUser(int libraryId, int articleId, [FromBody] AssignmentView assignment, CancellationToken token = default(CancellationToken))
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new AssignArticleToUserRequest(libraryId, articleId, assignment.AccountId);

        await _commandProcessor.SendAsync(request, cancellationToken: token);

        var renderResult = _articleRenderer.Render(request.Result, libraryId);

        return Ok(renderResult);
    }

    [HttpPost("libraries/{libraryId}/favorites/articles/{articleId}", Name = nameof(ArticleController.AddArticleToFavorites))]
    public async Task<IActionResult> AddArticleToFavorites(int libraryId, int articleId, CancellationToken token)
    {
        var request = new AddArticleToFavoriteRequest(libraryId, articleId, _userHelper.AccountId);
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        return new OkResult();
    }

    [HttpDelete("libraries/{libraryId}/favorites/articles/{articleId}", Name = nameof(ArticleController.RemoveArtiucleFromFavorites))]
    public async Task<IActionResult> RemoveArtiucleFromFavorites(int libraryId, int articleId, CancellationToken token)
    {
        var request = new RemoveArticleFromFavoriteRequest(libraryId, articleId, _userHelper.AccountId);
        await _commandProcessor.SendAsync(request, cancellationToken: token);

        return new OkResult();
    }
}
