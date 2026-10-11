using System.Globalization;
using System.Text;

namespace Inshapardaz.Domain.Helpers.Spelling;

// Folds the variant spellings that typing on different keyboards produces into one key, so that
// lookups and duplicate checks treat them as the same word: Arabic yeh/kaf/alef-maksura become the
// Urdu forms, diacritics (aerab), tatweel and zero-width joiners are dropped, Latin is lower-cased.
public static class WordNormaliser
{
    public static string Normalise(string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return string.Empty;

        var builder = new StringBuilder(word.Length);
        foreach (var c in word.Trim().Normalize(NormalizationForm.FormC))
        {
            switch (c)
            {
                case 'ي': // ARABIC LETTER YEH
                case 'ى': // ARABIC LETTER ALEF MAKSURA
                    builder.Append('ی'); // FARSI YEH
                    break;
                case 'ك': // ARABIC LETTER KAF
                    builder.Append('ک'); // KEHEH
                    break;
                case 'ـ': // TATWEEL
                case '‌': // ZERO WIDTH NON-JOINER
                case '‍': // ZERO WIDTH JOINER
                case '​': // ZERO WIDTH SPACE
                    break;
                default:
                    if (IsDiacritic(c)) break;
                    builder.Append(char.ToLowerInvariant(c));
                    break;
            }
        }

        return builder.ToString();
    }

    // True when the token has no letters (numbers, punctuation); such tokens are never misspelt.
    public static bool HasNoLetters(string word) => !word.Any(char.IsLetter);

    private static bool IsDiacritic(char c) =>
        c is >= 'ً' and <= 'ٟ' or 'ٰ'
        || CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark;
}
