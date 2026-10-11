using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Domain.Ports.Command.Tools.Dictionaries;
using Inshapardaz.Domain.Ports.Query.Tools.Dictionaries;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

// Meaning dictionaries: anyone may read and look words up; Admins and DictionaryWriters create
// dictionaries, and only a dictionary's owner (or an Admin) changes it.
public class DictionaryController(
    IAmACommandProcessor commandProcessor,
    IQueryProcessor queryProcessor,
    IRenderDictionary dictionaryRenderer)
    : Controller
{
    private const long MaxUploadBytes = 300L * 1024 * 1024;

    [HttpGet("/dictionaries", Name = nameof(GetDictionaries))]
    public async Task<IActionResult> GetDictionaries(string query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await queryProcessor.ExecuteAsync(new GetDictionariesQuery(query, pageNumber, pageSize), cancellationToken);
        return Ok(dictionaryRenderer.Render(new PageRendererArgs<DictionaryModel>
        {
            Page = result,
            RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query }
        }));
    }

    [HttpPost("/dictionaries", Name = nameof(AddDictionary))]
    public async Task<IActionResult> AddDictionary([FromBody] DictionaryView dictionary, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new AddDictionaryRequest(dictionary.Map());
        await commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

        var view = dictionaryRenderer.Render(request.Result);
        return new CreatedResult(view.Links.Self(), view);
    }

    // A StarDict/DSL dictionary as .zip or .tar.gz (or a single .dsl/.dsl.dz file). Name and
    // languages default to what the file's metadata says.
    [HttpPost("/dictionaries/upload", Name = nameof(ImportDictionary))]
    [RequestSizeLimit(MaxUploadBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxUploadBytes)]
    [RequestTimeout(600 * 1000)]
    public async Task<IActionResult> ImportDictionary(IFormFile file, [FromForm] string name, [FromForm] string sourceLanguage, [FromForm] string targetLanguage, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest();
        }

        await using var stream = file.OpenReadStream();
        var request = new ImportDictionaryRequest(stream, file.FileName)
        {
            Name = name,
            SourceLanguage = sourceLanguage,
            TargetLanguage = targetLanguage
        };
        await commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

        var view = dictionaryRenderer.Render(request.Result);
        return new CreatedResult(view.Links.Self(), view);
    }

    [HttpGet("/dictionaries/lookup", Name = nameof(LookupWord))]
    public async Task<IActionResult> LookupWord(string word, string dictionaryIds, CancellationToken cancellationToken)
    {
        var result = await queryProcessor.ExecuteAsync(new LookupWordQuery(word, ParseIds(dictionaryIds)), cancellationToken);
        return Ok(dictionaryRenderer.RenderArticles(result));
    }

    [HttpGet("/dictionaries/suggest", Name = nameof(SuggestHeadwords))]
    public async Task<IActionResult> SuggestHeadwords(string prefix, string dictionaryIds, int limit = 20, CancellationToken cancellationToken = default)
    {
        var result = await queryProcessor.ExecuteAsync(new SuggestHeadwordsQuery(prefix, ParseIds(dictionaryIds), limit), cancellationToken);
        return Ok(new HeadwordSuggestionsView { Suggestions = result });
    }

    [HttpGet("/dictionaries/{id:long}", Name = nameof(GetDictionaryById))]
    public async Task<IActionResult> GetDictionaryById(long id, CancellationToken cancellationToken)
    {
        var dictionary = await queryProcessor.ExecuteAsync(new GetDictionaryByIdQuery(id), cancellationToken);
        if (dictionary is null)
        {
            return NotFound();
        }

        return Ok(dictionaryRenderer.Render(dictionary));
    }

    [HttpPut("/dictionaries/{id:long}", Name = nameof(UpdateDictionary))]
    public async Task<IActionResult> UpdateDictionary(long id, [FromBody] DictionaryView dictionary, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var model = dictionary.Map();
        model.Id = id;
        var request = new UpdateDictionaryRequest(model);
        await commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

        return Ok(dictionaryRenderer.Render(request.Result));
    }

    [HttpDelete("/dictionaries/{id:long}", Name = nameof(DeleteDictionary))]
    public async Task<IActionResult> DeleteDictionary(long id, CancellationToken cancellationToken)
    {
        await commandProcessor.SendAsync(new DeleteDictionaryRequest(id), cancellationToken: cancellationToken);
        return NoContent();
    }

    [HttpGet("/dictionaries/{id:long}/export", Name = nameof(ExportDictionary))]
    [RequestTimeout(600 * 1000)]
    public async Task<IActionResult> ExportDictionary(long id, DictionaryExportFormat format = DictionaryExportFormat.StarDict, CancellationToken cancellationToken = default)
    {
        var file = await queryProcessor.ExecuteAsync(new ExportDictionaryQuery(id, format), cancellationToken);
        if (file is null)
        {
            return NotFound();
        }

        return File(file.Data, file.ContentType, file.FileName);
    }

    [HttpGet("/dictionaries/{id:long}/articles", Name = nameof(GetDictionaryArticles))]
    public async Task<IActionResult> GetDictionaryArticles(long id, string query, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var dictionary = await queryProcessor.ExecuteAsync(new GetDictionaryByIdQuery(id), cancellationToken);
        if (dictionary is null)
        {
            return NotFound();
        }

        var result = await queryProcessor.ExecuteAsync(new GetDictionaryArticlesQuery(id, query, pageNumber, pageSize), cancellationToken);
        return Ok(dictionaryRenderer.RenderArticles(new PageRendererArgs<DictionaryArticleModel>
        {
            Page = result,
            RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query }
        }, dictionary));
    }

    [HttpPost("/dictionaries/{id:long}/articles", Name = nameof(AddDictionaryArticle))]
    public async Task<IActionResult> AddDictionaryArticle(long id, [FromBody] DictionaryArticleView article, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var model = article.Map();
        model.DictionaryId = id;
        var request = new AddDictionaryArticleRequest(model);
        await commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

        var dictionary = await queryProcessor.ExecuteAsync(new GetDictionaryByIdQuery(id), cancellationToken);
        var view = dictionaryRenderer.RenderArticle(request.Result, dictionary);
        return new CreatedResult(view.Links.Self(), view);
    }

    [HttpGet("/dictionaries/{id:long}/articles/{articleId:long}", Name = nameof(GetDictionaryArticleById))]
    public async Task<IActionResult> GetDictionaryArticleById(long id, long articleId, CancellationToken cancellationToken)
    {
        var dictionary = await queryProcessor.ExecuteAsync(new GetDictionaryByIdQuery(id), cancellationToken);
        var article = dictionary is null ? null : await queryProcessor.ExecuteAsync(new GetDictionaryArticleByIdQuery(id, articleId), cancellationToken);
        if (article is null)
        {
            return NotFound();
        }

        return Ok(dictionaryRenderer.RenderArticle(article, dictionary));
    }

    [HttpPut("/dictionaries/{id:long}/articles/{articleId:long}", Name = nameof(UpdateDictionaryArticle))]
    public async Task<IActionResult> UpdateDictionaryArticle(long id, long articleId, [FromBody] DictionaryArticleView article, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var model = article.Map();
        model.DictionaryId = id;
        model.Id = articleId;
        var request = new UpdateDictionaryArticleRequest(model);
        await commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

        var dictionary = await queryProcessor.ExecuteAsync(new GetDictionaryByIdQuery(id), cancellationToken);
        return Ok(dictionaryRenderer.RenderArticle(request.Result, dictionary));
    }

    [HttpDelete("/dictionaries/{id:long}/articles/{articleId:long}", Name = nameof(DeleteDictionaryArticle))]
    public async Task<IActionResult> DeleteDictionaryArticle(long id, long articleId, CancellationToken cancellationToken)
    {
        await commandProcessor.SendAsync(new DeleteDictionaryArticleRequest(id, articleId), cancellationToken: cancellationToken);
        return NoContent();
    }

    [HttpGet("/dictionaries/{id:long}/merge-candidates", Name = nameof(GetMergeCandidates))]
    public async Task<IActionResult> GetMergeCandidates(long id, int limit = 100, CancellationToken cancellationToken = default)
    {
        var dictionary = await queryProcessor.ExecuteAsync(new GetDictionaryByIdQuery(id), cancellationToken);
        if (dictionary is null)
        {
            return NotFound();
        }

        var groups = await queryProcessor.ExecuteAsync(new GetMergeCandidatesQuery(id, limit), cancellationToken);
        return Ok(groups.Select(g => new MergeCandidateGroupView
        {
            BaseHeadword = g.BaseHeadword,
            Articles = g.Articles.Select(a => dictionaryRenderer.RenderArticle(a, dictionary)).ToList()
        }));
    }

    [HttpPost("/dictionaries/{id:long}/merge", Name = nameof(MergeDictionaryArticles))]
    public async Task<IActionResult> MergeDictionaryArticles(long id, [FromBody] MergeArticlesView merge, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new MergeDictionaryArticlesRequest(id, merge.ArticleIds, merge.Headword);
        await commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

        var dictionary = await queryProcessor.ExecuteAsync(new GetDictionaryByIdQuery(id), cancellationToken);
        return Ok(dictionaryRenderer.RenderArticle(request.Result, dictionary));
    }

    // "1,2,3" -> [1, 2, 3]; null (all dictionaries) when empty. Ignores entries that aren't numbers.
    private static IEnumerable<long> ParseIds(string ids) =>
        string.IsNullOrWhiteSpace(ids)
            ? null
            : ids.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => long.TryParse(s, out var id) ? id : (long?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();
}
