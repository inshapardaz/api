using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Ports.Command.Tools.Spelling;
using Inshapardaz.Domain.Ports.Query.Tools.Spelling;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

public class SpellCheckerController(
    IAmACommandProcessor commandProcessor,
    IQueryProcessor queryProcessor)
    : Controller
{
    [HttpPost("/tools/{language}/spellchecker/check", Name = nameof(CheckSpelling))]
    public async Task<IActionResult> CheckSpelling(string language, [FromBody] SpellCheckRequestView request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var result = await queryProcessor.ExecuteAsync(new CheckSpellingQuery(language, request.Words), cancellationToken);
        return Ok(result.Select(r => new SpellCheckResultView
        {
            Word = r.Word,
            Found = r.Found,
            Suggestions = r.Suggestions.Select(s => new SpellingSuggestionView { Word = s.Word, Priority = s.Priority })
        }));
    }

    [HttpPost("/tools/{language}/spellchecker/replace", Name = nameof(RecordSpellingReplacement))]
    public async Task<IActionResult> RecordSpellingReplacement(string language, [FromBody] SpellingReplacementView replacement, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        await commandProcessor.SendAsync(new RecordSpellingReplacementRequest(language, replacement.Word, replacement.Replacement), cancellationToken: cancellationToken);
        return NoContent();
    }
}
