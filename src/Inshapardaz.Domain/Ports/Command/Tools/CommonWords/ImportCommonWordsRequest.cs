using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Helpers.Spelling;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools.CommonWords;

// Adds many words to a language's spelling dictionary at once, skipping blanks and words that are
// already there (compared in normalised form).
public class ImportCommonWordsRequest(string language, IEnumerable<string> words) : RequestBase
{
    public string Language { get; } = language;
    public IEnumerable<string> Words { get; } = words;
    public WordImportResultModel Result { get; set; }
}

public class ImportCommonWordsRequestHandler(ICommonWordsRepository commonWordsRepository, SpellingIndexCache spellingIndexCache)
    : RequestHandlerAsync<ImportCommonWordsRequest>
{
    [AuthorizeLanguageTools(1)]
    public override async Task<ImportCommonWordsRequest> HandleAsync(ImportCommonWordsRequest command, CancellationToken cancellationToken = default)
    {
        var existing = new SpellingIndex(await commonWordsRepository.GetWordsForLanguage(command.Language, cancellationToken));
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var toAdd = new List<string>();
        var skipped = 0;

        foreach (var word in command.Words ?? [])
        {
            var trimmed = word?.Trim();
            var normalised = WordNormaliser.Normalise(trimmed);
            if (normalised.Length == 0 || existing.Contains(trimmed) || !seen.Add(normalised))
            {
                skipped++;
                continue;
            }
            toAdd.Add(trimmed);
        }

        if (toAdd.Count > 0)
        {
            await commonWordsRepository.AddWords(command.Language, toAdd, cancellationToken);
            spellingIndexCache.Invalidate(command.Language);
        }

        command.Result = new WordImportResultModel { Added = toAdd.Count, Skipped = skipped };
        return await base.HandleAsync(command, cancellationToken);
    }
}
