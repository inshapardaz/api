using System.Text;
using Inshapardaz.Adapters.Transliteration.Mappings;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Transliteration;

namespace Inshapardaz.Adapters.Transliteration;

// Dictionary-first transliteration, ported from LanguageTools' DictionaryTransliterator: Roman input
// is matched against the dictionary (three- and two-word phrases first, then single words with their
// surrounding punctuation kept aside) and falls back to the rule-based engine; whitespace is preserved.
public class Transliterator : ITransliterate
{
    private const int MaxPhraseWords = 3;

    private readonly ScriptTransliterator _rules = ScriptTransliterator.Instance;

    public string Convert(string text, TransliterationScript from, TransliterationScript to)
    {
        if (string.IsNullOrEmpty(text) || from == to) return text ?? string.Empty;
        return _rules.Convert(text, Map(from), Map(to));
    }

    public string ConvertWithDictionary(string text, TransliterationScript from, TransliterationScript to, Func<string, string> lookupNative)
    {
        if (string.IsNullOrEmpty(text) || from == to) return text ?? string.Empty;
        if (from != TransliterationScript.Roman) return Convert(text, from, to);

        var tokens = TokenizePreservingWhitespace(text);
        var words = tokens.Where(t => !t.IsWhitespace).Select(t => t.Text).ToArray();

        // Phrases first, so a phrase wins over its individual words.
        var phrases = new Dictionary<int, (string Native, int Length)>();
        var consumed = new HashSet<int>();
        for (var i = 0; i < words.Length; i++)
        {
            if (consumed.Contains(i)) continue;
            for (var length = Math.Min(MaxPhraseWords, words.Length - i); length > 1; length--)
            {
                var native = Lookup(string.Join(' ', words.Skip(i).Take(length)), lookupNative);
                if (native == null) continue;

                phrases[i] = (native, length);
                for (var c = 0; c < length; c++) consumed.Add(i + c);
                break;
            }
        }

        var result = new StringBuilder();
        var wordIndex = 0;
        var phraseEnd = 0;
        foreach (var token in tokens)
        {
            if (token.IsWhitespace)
            {
                // Whitespace inside a matched phrase is replaced along with its words. (The
                // LanguageTools original kept it, leaving stray spaces after each phrase.)
                if (wordIndex >= phraseEnd) result.Append(token.Text);
                continue;
            }

            if (phrases.TryGetValue(wordIndex, out var phrase))
            {
                result.Append(phrase.Native);
                phraseEnd = wordIndex + phrase.Length;
            }
            else if (!consumed.Contains(wordIndex))
            {
                var (leading, core, trailing) = SplitPunctuation(token.Text);
                result.Append(leading);
                if (core.Length > 0)
                {
                    result.Append(Lookup(core, lookupNative) ?? _rules.Convert(core, Script.Roman, Map(to)));
                }
                result.Append(trailing);
            }

            wordIndex++;
        }

        return result.ToString();
    }

    // Separates leading and trailing punctuation from the word between them.
    private static (string Leading, string Core, string Trailing) SplitPunctuation(string token)
    {
        int start = 0, end = token.Length;
        while (start < end && char.IsPunctuation(token[start])) start++;
        while (end > start && char.IsPunctuation(token[end - 1])) end--;
        return start >= end ? (token, string.Empty, string.Empty) : (token[..start], token[start..end], token[end..]);
    }

    private static string Lookup(string roman, Func<string, string> lookupNative)
    {
        var (_, core, _) = SplitPunctuation(roman.Trim());
        return core.Length == 0 ? null : lookupNative(core.ToLowerInvariant());
    }

    private static Script Map(TransliterationScript script) => script switch
    {
        TransliterationScript.Roman => Script.Roman,
        TransliterationScript.UrduArabic => Script.UrduArabic,
        TransliterationScript.HindiDevanagari => Script.HindiDevanagari,
        _ => throw new ArgumentOutOfRangeException(nameof(script))
    };

    private static List<(string Text, bool IsWhitespace)> TokenizePreservingWhitespace(string input)
    {
        var tokens = new List<(string Text, bool IsWhitespace)>();
        var i = 0;
        while (i < input.Length)
        {
            var start = i;
            var whitespace = char.IsWhiteSpace(input[i]);
            while (i < input.Length && char.IsWhiteSpace(input[i]) == whitespace) i++;
            tokens.Add((input[start..i], whitespace));
        }
        return tokens;
    }
}
