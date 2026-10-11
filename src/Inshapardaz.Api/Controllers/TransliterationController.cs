using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models.Transliteration;
using Inshapardaz.Domain.Ports.Command.Tools.Transliteration;
using Inshapardaz.Domain.Ports.Query.Tools.Transliteration;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

// Script conversion (Roman / Urdu / Hindi) and each language's transliteration dictionary.
public class TransliterationController(
    IAmACommandProcessor commandProcessor,
    IQueryProcessor queryProcessor,
    IRenderTransliteration transliterationRenderer)
    : Controller
{
    [HttpPost("/tools/{language}/transliterate", Name = nameof(Transliterate))]
    public async Task<IActionResult> Transliterate(string language, [FromBody] TransliterateRequestView request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var result = await queryProcessor.ExecuteAsync(new TransliterateQuery(language, request.Text, request.From, request.To, request.Details), cancellationToken);
        return Ok(transliterationRenderer.Render(result));
    }

    [HttpGet("/tools/{language}/transliteration/entries", Name = nameof(GetTransliterationEntries))]
    public async Task<IActionResult> GetTransliterationEntries(string language, string query, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await queryProcessor.ExecuteAsync(new GetTransliterationEntriesQuery(language, query, pageNumber, pageSize), cancellationToken);
        return Ok(transliterationRenderer.Render(new PageRendererArgs<TransliterationEntryModel>
        {
            Page = result,
            RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query }
        }, language));
    }

    [HttpGet("/tools/{language}/transliteration/entries/{id:long}", Name = nameof(GetTransliterationEntryById))]
    public async Task<IActionResult> GetTransliterationEntryById(string language, long id, CancellationToken cancellationToken)
    {
        var entry = await queryProcessor.ExecuteAsync(new GetTransliterationEntryByIdQuery(language, id), cancellationToken);
        return entry is null ? NotFound() : Ok(transliterationRenderer.Render(entry));
    }

    [HttpPost("/tools/{language}/transliteration/entries", Name = nameof(AddTransliterationEntry))]
    public async Task<IActionResult> AddTransliterationEntry(string language, [FromBody] TransliterationEntryView entry, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var request = new AddTransliterationEntryRequest(Map(entry, language));
        await commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

        var view = transliterationRenderer.Render(request.Result);
        return new CreatedResult(view.Links.Self(), view);
    }

    [HttpPut("/tools/{language}/transliteration/entries/{id:long}", Name = nameof(UpdateTransliterationEntry))]
    public async Task<IActionResult> UpdateTransliterationEntry(string language, long id, [FromBody] TransliterationEntryView entry, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var model = Map(entry, language);
        model.Id = id;
        var request = new UpdateTransliterationEntryRequest(model);
        await commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

        return Ok(transliterationRenderer.Render(request.Result));
    }

    [HttpDelete("/tools/{language}/transliteration/entries/{id:long}", Name = nameof(DeleteTransliterationEntry))]
    public async Task<IActionResult> DeleteTransliterationEntry(string language, long id, CancellationToken cancellationToken)
    {
        await commandProcessor.SendAsync(new DeleteTransliterationEntryRequest(language, id), cancellationToken: cancellationToken);
        return NoContent();
    }

    [HttpPost("/tools/{language}/transliteration/entries/bulk", Name = nameof(ImportTransliterationEntries))]
    public async Task<IActionResult> ImportTransliterationEntries(string language, [FromBody] IEnumerable<TransliterationEntryView> entries, CancellationToken cancellationToken)
    {
        var request = new ImportTransliterationEntriesRequest(language, (entries ?? []).Select(e => Map(e, language)).ToList());
        await commandProcessor.SendAsync(request, cancellationToken: cancellationToken);
        return Ok(new TransliterationImportResultView { Added = request.Result.Added, Updated = request.Result.Updated, Skipped = request.Result.Skipped });
    }

    [HttpGet("/tools/{language}/transliteration/entries/export", Name = nameof(ExportTransliterationEntries))]
    public async Task<IActionResult> ExportTransliterationEntries(string language, CancellationToken cancellationToken)
    {
        var entries = await queryProcessor.ExecuteAsync(new GetAllTransliterationEntriesQuery(language), cancellationToken);
        var json = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(
            entries.Select(e => new { e.Roman, e.Native, e.Meaning, e.Category }),
            new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web) { WriteIndented = true });
        return File(json, "application/json", $"{language}-transliteration.json");
    }

    private static TransliterationEntryModel Map(TransliterationEntryView view, string language) => new()
    {
        Id = view?.Id ?? 0,
        Language = language,
        Roman = view?.Roman,
        Native = view?.Native,
        Meaning = view?.Meaning,
        Category = view?.Category
    };
}
