using System.Collections.Concurrent;

namespace Inshapardaz.Domain.Helpers.Spelling;

// Process-wide cache of one SpellingIndex per language (registered as a singleton). Building an
// index means reading the whole word list, so it is built on first use and dropped whenever that
// language's words change. With several API instances, each keeps its own copy and only sees
// another instance's edits once its own copy is invalidated -- acceptable for the single-container
// deployment.
public class SpellingIndexCache
{
    private readonly ConcurrentDictionary<string, Lazy<Task<SpellingIndex>>> _indexes = new(StringComparer.OrdinalIgnoreCase);

    public async Task<SpellingIndex> GetAsync(string language, Func<Task<IEnumerable<string>>> loadWords)
    {
        var entry = _indexes.GetOrAdd(language, _ => new Lazy<Task<SpellingIndex>>(async () => new SpellingIndex(await loadWords())));
        try
        {
            return await entry.Value;
        }
        catch
        {
            // Don't cache a failed load (e.g. a transient database error).
            _indexes.TryRemove(new KeyValuePair<string, Lazy<Task<SpellingIndex>>>(language, entry));
            throw;
        }
    }

    public void Invalidate(string language) => _indexes.TryRemove(language, out _);
}
