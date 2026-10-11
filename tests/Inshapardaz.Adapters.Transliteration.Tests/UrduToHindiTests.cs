using Inshapardaz.Adapters.Transliteration.Mappings;

namespace Inshapardaz.Adapters.Transliteration.Tests;

public class UrduToHindiTests
{
    private readonly ScriptTransliterator _transliterator = ScriptTransliterator.Instance;

    [TestCase("\u0628", "\u092C")]       // ب → ब
    [TestCase("\u067E", "\u092A")]       // پ → प
    [TestCase("\u062A", "\u0924")]       // ت → त
    [TestCase("\u062C", "\u091C")]       // ج → ज
    [TestCase("\u0686", "\u091A")]       // چ → च
    [TestCase("\u062F", "\u0926")]       // د → द
    [TestCase("\u0631", "\u0930")]       // ر → र
    [TestCase("\u0633", "\u0938")]       // س → स
    [TestCase("\u0634", "\u0936")]       // ش → श
    [TestCase("\u06A9", "\u0915")]       // ک → क
    [TestCase("\u06AF", "\u0917")]       // گ → ग
    [TestCase("\u0644", "\u0932")]       // ل → ल
    [TestCase("\u0645", "\u092E")]       // م → म
    [TestCase("\u0646", "\u0928")]       // ن → न
    public void ShouldConvertBasicConsonants(string urdu, string expectedHindi)
    {
        var result = _transliterator.UrduToHindi(urdu);
        Assert.That(result, Is.EqualTo(expectedHindi));
    }

    [TestCase("\u0679", "\u091F")]       // ٹ → ट
    [TestCase("\u0688", "\u0921")]       // ڈ → ड
    [TestCase("\u0691", "\u0921\u093C")] // ڑ → ड़
    public void ShouldConvertRetroflexConsonants(string urdu, string expectedHindi)
    {
        var result = _transliterator.UrduToHindi(urdu);
        Assert.That(result, Is.EqualTo(expectedHindi));
    }

    [TestCase("\u06F0", "\u0966")]   // ۰ → ०
    [TestCase("\u06F1", "\u0967")]   // ۱ → १
    [TestCase("\u06F5", "\u096B")]   // ۵ → ५
    [TestCase("\u06F9", "\u096F")]   // ۹ → ९
    public void ShouldConvertNumerals(string urdu, string expectedHindi)
    {
        var result = _transliterator.UrduToHindi(urdu);
        Assert.That(result, Is.EqualTo(expectedHindi));
    }

    [TestCase("\u0642", "\u0915\u093C")]       // ق → क़
    [TestCase("\u063A", "\u0917\u093C")]       // غ → ग़
    [TestCase("\u0641", "\u092B\u093C")]       // ف → फ़
    public void ShouldConvertArabicSpecificLetters(string urdu, string expectedHindi)
    {
        var result = _transliterator.UrduToHindi(urdu);
        Assert.That(result, Is.EqualTo(expectedHindi));
    }

    [Test]
    public void ShouldReturnEmptyForEmptyInput()
    {
        Assert.That(_transliterator.UrduToHindi(""), Is.EqualTo(string.Empty));
    }
}
