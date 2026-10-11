namespace Inshapardaz.Domain.Helpers.Spelling;

// Reads an uploaded word list: plain text with one word per line, or a Hunspell .dic file (an
// optional leading word count, then "word/AFFIXFLAGS" lines -- the flags are dropped).
public static class WordListParser
{
    public static IEnumerable<string> Parse(TextReader reader)
    {
        var first = true;
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            var text = line.Trim();
            if (first)
            {
                first = false;
                if (int.TryParse(text, out _)) continue;
            }

            if (text.Length == 0 || text.StartsWith('#')) continue;

            var slash = text.IndexOf('/');
            if (slash == 0) continue;
            if (slash > 0) text = text[..slash];

            // Hunspell entries may carry morphological fields after whitespace.
            var space = text.IndexOfAny([' ', '\t']);
            if (space > 0) text = text[..space];

            yield return text;
        }
    }
}
