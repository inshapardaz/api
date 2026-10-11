using Inshapardaz.Domain.Models.Transliteration;

namespace Inshapardaz.Domain.Adapters;

// Script conversion between romanised text, Urdu (Arabic script) and Hindi (Devanagari).
public interface ITransliterate
{
    // Rule-based conversion only.
    string Convert(string text, TransliterationScript from, TransliterationScript to);

    // Roman input is first matched against the dictionary (phrases of up to three words, then single
    // words, ignoring surrounding punctuation); lookupNative returns the native spelling for a
    // lower-cased romanised word or phrase, or null. Other directions are rule-based.
    string ConvertWithDictionary(string text, TransliterationScript from, TransliterationScript to, Func<string, string> lookupNative);
}
