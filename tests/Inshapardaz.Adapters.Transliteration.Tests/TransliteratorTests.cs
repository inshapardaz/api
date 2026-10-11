using Inshapardaz.Domain.Models.Transliteration;

namespace Inshapardaz.Adapters.Transliteration.Tests;

public class TransliteratorTests
{
    private readonly Transliterator _transliterator = new();

    private static readonly Dictionary<string, string> Entries = new()
    {
        ["khuda"] = "خدا",
        ["assalam o alaikum"] = "السلام علیکم",
        ["o"] = "او",
    };

    private string Convert(string text, TransliterationScript to = TransliterationScript.UrduArabic) =>
        _transliterator.ConvertWithDictionary(text, TransliterationScript.Roman, to, roman => Entries.GetValueOrDefault(roman));

    [Test]
    public void UsesDictionarySpellingForKnownWordsIgnoringCase() => Assert.That(Convert("Khuda"), Is.EqualTo("خدا"));

    [Test]
    public void KeepsPunctuationAroundDictionaryWords() => Assert.That(Convert("(khuda),"), Is.EqualTo("(خدا),"));

    [Test]
    public void PrefersPhrasesOverTheirWords() => Assert.That(Convert("assalam o alaikum"), Is.EqualTo("السلام علیکم"));

    [Test]
    public void KeepsSpacingAroundPhrases() => Assert.That(Convert("khuda  assalam o alaikum khuda"), Is.EqualTo("خدا  السلام علیکم خدا"));

    [Test]
    public void PreservesWhitespace() => Assert.That(Convert("khuda\n\tkhuda"), Is.EqualTo("خدا\n\tخدا"));

    [Test]
    public void FallsBackToRulesForUnknownWords() =>
        Assert.That(Convert("dil"), Is.EqualTo(ScriptTransliterator.Instance.RomanToUrdu("dil")));

    [Test]
    public void IgnoresDictionaryForNonRomanInput() =>
        Assert.That(_transliterator.ConvertWithDictionary("دل", TransliterationScript.UrduArabic, TransliterationScript.Roman, _ => "never"),
            Is.EqualTo(ScriptTransliterator.Instance.UrduToRoman("دل")));

    [Test]
    public void ReturnsTextUnchangedForSameScript() => Assert.That(Convert("khuda", TransliterationScript.Roman), Is.EqualTo("khuda"));
}
