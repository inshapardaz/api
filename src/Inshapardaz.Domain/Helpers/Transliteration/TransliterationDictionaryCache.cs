using System.Collections.Concurrent;
using Inshapardaz.Domain.Models.Transliteration;

namespace Inshapardaz.Domain.Helpers.Transliteration;

// Process-wide cache of each language's transliteration entries, keyed by lower-cased roman form
// (registered as a singleton). Loaded on first use and dropped whenever that language's entries change.
public class TransliterationDictionaryCache
{
    private readonly ConcurrentDictionary<string, Lazy<Task<IReadOnlyDictionary<string, TransliterationEntryModel>>>> _entries = new(StringComparer.OrdinalIgnoreCase);

    public async Task<IReadOnlyDictionary<string, TransliterationEntryModel>> GetAsync(string language, Func<Task<IEnumerable<TransliterationEntryModel>>> loadEntries)
    {
        var entry = _entries.GetOrAdd(language, _ => new Lazy<Task<IReadOnlyDictionary<string, TransliterationEntryModel>>>(async () =>
        {
            var byRoman = new Dictionary<string, TransliterationEntryModel>(StringComparer.Ordinal);
            foreach (var item in await loadEntries())
            {
                byRoman.TryAdd(RomanKey(item.Roman), item);
            }
            return byRoman;
        }));

        try
        {
            return await entry.Value;
        }
        catch
        {
            _entries.TryRemove(new KeyValuePair<string, Lazy<Task<IReadOnlyDictionary<string, TransliterationEntryModel>>>>(language, entry));
            throw;
        }
    }

    public void Invalidate(string language) => _entries.TryRemove(language, out _);

    public static string RomanKey(string roman) => string.Join(' ', (roman ?? string.Empty).Trim().ToLowerInvariant().Split((char[])null, StringSplitOptions.RemoveEmptyEntries));
}
