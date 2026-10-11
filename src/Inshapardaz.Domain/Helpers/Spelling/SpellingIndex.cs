namespace Inshapardaz.Domain.Helpers.Spelling;

// In-memory index over one language's word list, answering "is this a word?" and "what did the
// writer probably mean?". Suggestions use symmetric-delete matching (as in SymSpell): every word is
// indexed under each string obtainable by deleting up to MaxEditDistance characters, so a lookup
// only generates the deletes of the input instead of comparing against every word. Only the first
// PrefixLength characters are indexed (candidates are still checked against the whole word), which
// keeps memory bounded for long words at little cost in accuracy.
public class SpellingIndex
{
    public const int MaxEditDistance = 2;
    private const int PrefixLength = 7;

    // Normalised form -> word as stored (first one wins when several normalise alike).
    private readonly Dictionary<string, string> _words = new(StringComparer.Ordinal);
    private readonly Dictionary<string, List<string>> _deletes = new(StringComparer.Ordinal);

    public SpellingIndex(IEnumerable<string> words)
    {
        foreach (var word in words)
        {
            var normalised = WordNormaliser.Normalise(word);
            if (normalised.Length == 0 || !_words.TryAdd(normalised, word.Trim())) continue;

            foreach (var delete in Deletes(Prefix(normalised)))
            {
                if (!_deletes.TryGetValue(delete, out var bucket))
                {
                    _deletes[delete] = bucket = new List<string>(1);
                }
                bucket.Add(normalised);
            }
        }
    }

    public int Count => _words.Count;

    public bool Contains(string word) => _words.ContainsKey(WordNormaliser.Normalise(word));

    // Closest words first (by edit distance, then alphabetically), as stored in the dictionary.
    public IReadOnlyList<string> Suggest(string word, int limit)
    {
        var input = WordNormaliser.Normalise(word);
        if (input.Length == 0 || limit <= 0) return [];

        var candidates = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var delete in Deletes(Prefix(input)))
        {
            if (!_deletes.TryGetValue(delete, out var bucket)) continue;

            foreach (var candidate in bucket)
            {
                if (candidates.ContainsKey(candidate)) continue;
                if (Math.Abs(candidate.Length - input.Length) > MaxEditDistance) continue;

                var distance = DamerauLevenshtein(input, candidate, MaxEditDistance);
                if (distance >= 0 && distance <= MaxEditDistance)
                {
                    candidates[candidate] = distance;
                }
            }
        }

        return candidates
            .Where(c => c.Value > 0)
            .OrderBy(c => c.Value)
            .ThenBy(c => c.Key, StringComparer.Ordinal)
            .Take(limit)
            .Select(c => _words[c.Key])
            .ToList();
    }

    private static string Prefix(string word) => word.Length > PrefixLength ? word[..PrefixLength] : word;

    // The word itself plus every string reachable by deleting 1..MaxEditDistance characters.
    private static HashSet<string> Deletes(string word)
    {
        var result = new HashSet<string>(StringComparer.Ordinal) { word };
        var frontier = new List<string> { word };
        for (var distance = 0; distance < MaxEditDistance; distance++)
        {
            var next = new List<string>();
            foreach (var item in frontier)
            {
                if (item.Length <= 1) continue;
                for (var i = 0; i < item.Length; i++)
                {
                    var delete = item.Remove(i, 1);
                    if (result.Add(delete)) next.Add(delete);
                }
            }
            frontier = next;
        }
        return result;
    }

    // Optimal string alignment distance; returns -1 once it is certain to exceed maxDistance.
    private static int DamerauLevenshtein(string a, string b, int maxDistance)
    {
        var previousPrevious = new int[b.Length + 1];
        var previous = new int[b.Length + 1];
        var current = new int[b.Length + 1];
        for (var j = 0; j <= b.Length; j++) previous[j] = j;

        for (var i = 1; i <= a.Length; i++)
        {
            current[0] = i;
            var rowMinimum = current[0];
            for (var j = 1; j <= b.Length; j++)
            {
                var cost = a[i - 1] == b[j - 1] ? 0 : 1;
                var value = Math.Min(Math.Min(previous[j] + 1, current[j - 1] + 1), previous[j - 1] + cost);
                if (i > 1 && j > 1 && a[i - 1] == b[j - 2] && a[i - 2] == b[j - 1])
                {
                    value = Math.Min(value, previousPrevious[j - 2] + 1);
                }
                current[j] = value;
                rowMinimum = Math.Min(rowMinimum, value);
            }

            if (rowMinimum > maxDistance) return -1;
            (previousPrevious, previous, current) = (previous, current, previousPrevious);
        }

        return previous[b.Length];
    }
}
