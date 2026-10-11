using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers.Spelling;
using Inshapardaz.Domain.Models;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Tools.Spelling;

public class CheckSpellingQuery(string language, IEnumerable<string> words) : IQuery<IEnumerable<SpellCheckResultModel>>
{
    public const int MaxWords = 2000;
    public const int MaxSuggestions = 8;

    public string Language { get; } = language;
    public IEnumerable<string> Words { get; } = words;
}

// Checks each distinct word against the language's spelling dictionary. Unknown words get
// suggestions: replacements writers chose before (most chosen first), then the closest
// dictionary words.
public class CheckSpellingQueryHandler(
    ICommonWordsRepository commonWordsRepository,
    ISpellingReplacementRepository replacementRepository,
    SpellingIndexCache spellingIndexCache)
    : QueryHandlerAsync<CheckSpellingQuery, IEnumerable<SpellCheckResultModel>>
{
    public override async Task<IEnumerable<SpellCheckResultModel>> ExecuteAsync(CheckSpellingQuery query, CancellationToken cancellationToken = default)
    {
        var words = (query.Words ?? [])
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .Select(w => w.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (words.Count > CheckSpellingQuery.MaxWords)
        {
            throw new BadRequestException($"At most {CheckSpellingQuery.MaxWords} words can be checked in one request.");
        }

        var index = await spellingIndexCache.GetAsync(query.Language,
            () => commonWordsRepository.GetWordsForLanguage(query.Language, cancellationToken));

        var unknown = words.Where(w => !WordNormaliser.HasNoLetters(w) && !index.Contains(w)).ToList();
        var history = (await replacementRepository.GetReplacements(query.Language, unknown.Select(WordNormaliser.Normalise), cancellationToken))
            .GroupBy(r => r.SourceWord, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.Ordinal);

        return words.Select(word =>
        {
            if (!unknown.Contains(word))
            {
                return new SpellCheckResultModel { Word = word, Found = true };
            }

            var suggestions = new List<SpellingSuggestionModel>();
            if (history.TryGetValue(WordNormaliser.Normalise(word), out var previous))
            {
                suggestions.AddRange(previous.Select(r => new SpellingSuggestionModel { Word = r.Replacement, Priority = r.Count }));
            }

            foreach (var suggestion in index.Suggest(word, CheckSpellingQuery.MaxSuggestions))
            {
                if (suggestions.All(s => s.Word != suggestion))
                {
                    suggestions.Add(new SpellingSuggestionModel { Word = suggestion });
                }
            }

            return new SpellCheckResultModel
            {
                Word = word,
                Found = false,
                Suggestions = suggestions.Take(CheckSpellingQuery.MaxSuggestions).ToList()
            };
        }).ToList();
    }
}
