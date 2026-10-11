using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers.Transliteration;
using Inshapardaz.Domain.Models.Transliteration;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Tools.Transliteration;

// Converts text between scripts using the language's transliteration dictionary for Roman input.
// With Details, also returns a word-by-word breakdown saying where each conversion came from.
public class TransliterateQuery(string language, string text, TransliterationScript from, TransliterationScript to, bool details) : IQuery<TransliterationResultModel>
{
    public const int MaxLength = 100_000;

    public string Language { get; } = language;
    public string Text { get; } = text;
    public TransliterationScript From { get; } = from;
    public TransliterationScript To { get; } = to;
    public bool Details { get; } = details;
}

public class TransliterateQueryHandler(
    ITransliterationRepository transliterationRepository,
    TransliterationDictionaryCache dictionaryCache,
    ITransliterate transliterator)
    : QueryHandlerAsync<TransliterateQuery, TransliterationResultModel>
{
    public override async Task<TransliterationResultModel> ExecuteAsync(TransliterateQuery query, CancellationToken cancellationToken = default)
    {
        if (!TransliterationLanguages.TryGetScript(query.Language, out var languageScript))
        {
            throw new BadRequestException($"Transliteration is not available for '{query.Language}'. Available: {string.Join(", ", TransliterationLanguages.All)}.");
        }

        if (string.IsNullOrWhiteSpace(query.Text))
        {
            throw new BadRequestException("Text is required.");
        }

        if (query.Text.Length > TransliterateQuery.MaxLength)
        {
            throw new BadRequestException($"Text can be at most {TransliterateQuery.MaxLength} characters.");
        }

        var entries = await dictionaryCache.GetAsync(query.Language,
            () => transliterationRepository.GetAllEntries(query.Language, cancellationToken));
        // The dictionary holds native spellings in the language's own script, so it only helps when
        // converting into that script.
        if (query.To != languageScript)
        {
            entries = new Dictionary<string, TransliterationEntryModel>();
        }
        string Lookup(string roman) => entries.TryGetValue(roman, out var entry) ? entry.Native : null;

        var result = new TransliterationResultModel
        {
            From = query.From,
            To = query.To,
            Result = transliterator.ConvertWithDictionary(query.Text, query.From, query.To, Lookup)
        };

        if (query.Details && query.From != query.To)
        {
            result.Words = Breakdown(query, entries, Lookup);
        }

        return result;
    }

    // Each whitespace-separated token, with leading/trailing punctuation reported separately and
    // the dictionary entry (if any) that the word matched.
    private List<TransliteratedWordModel> Breakdown(TransliterateQuery query, IReadOnlyDictionary<string, TransliterationEntryModel> entries, Func<string, string> lookup)
    {
        var words = new List<TransliteratedWordModel>();
        foreach (var token in query.Text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries))
        {
            var (leading, core, trailing) = SplitPunctuation(token);
            if (leading.Length > 0) words.Add(Punctuation(leading));

            if (core.Length > 0)
            {
                var roman = query.From == TransliterationScript.Roman ? core : transliterator.Convert(core, query.From, TransliterationScript.Roman);
                entries.TryGetValue(TransliterationDictionaryCache.RomanKey(roman), out var entry);
                words.Add(new TransliteratedWordModel
                {
                    Input = core,
                    Output = transliterator.ConvertWithDictionary(core, query.From, query.To, lookup),
                    Source = entry != null ? "dictionary" : "rules",
                    EntryId = entry?.Id
                });
            }

            if (trailing.Length > 0) words.Add(Punctuation(trailing));
        }
        return words;
    }

    private static TransliteratedWordModel Punctuation(string text) => new() { Input = text, Output = text, Source = "punctuation" };

    private static (string Leading, string Core, string Trailing) SplitPunctuation(string token)
    {
        int start = 0, end = token.Length;
        while (start < end && char.IsPunctuation(token[start])) start++;
        while (end > start && char.IsPunctuation(token[end - 1])) end--;
        return start >= end ? (token, string.Empty, string.Empty) : (token[..start], token[start..end], token[end..]);
    }
}
