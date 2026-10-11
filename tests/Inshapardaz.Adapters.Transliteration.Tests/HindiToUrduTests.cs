using Inshapardaz.Adapters.Transliteration.Mappings;

namespace Inshapardaz.Adapters.Transliteration.Tests;

public class HindiToUrduTests
{
    private readonly ScriptTransliterator _transliterator = ScriptTransliterator.Instance;

    [TestCase("\u092C", "\u0628")]       // ब → ب
    [TestCase("\u092A", "\u067E")]       // प → پ
    [TestCase("\u0924", "\u062A")]       // त → ت
    [TestCase("\u091C", "\u062C")]       // ज → ج
    [TestCase("\u091A", "\u0686")]       // च → چ
    [TestCase("\u0926", "\u062F")]       // द → د
    [TestCase("\u0930", "\u0631")]       // र → ر
    [TestCase("\u0938", "\u0633")]       // स → س
    [TestCase("\u0936", "\u0634")]       // श → ش
    [TestCase("\u0915", "\u06A9")]       // क → ک
    [TestCase("\u0917", "\u06AF")]       // ग → گ
    [TestCase("\u0932", "\u0644")]       // ल → ل
    [TestCase("\u092E", "\u0645")]       // म → م
    [TestCase("\u0928", "\u0646")]       // न → ن
    public void ShouldConvertBasicConsonants(string hindi, string expectedUrdu)
    {
        var result = _transliterator.HindiToUrdu(hindi);
        Assert.That(result, Is.EqualTo(expectedUrdu));
    }

    [TestCase("\u091F", "\u0679")]       // ट → ٹ
    [TestCase("\u0921", "\u0688")]       // ड → ڈ
    [TestCase("\u0921\u093C", "\u0691")] // ड़ → ڑ
    public void ShouldConvertRetroflexConsonants(string hindi, string expectedUrdu)
    {
        var result = _transliterator.HindiToUrdu(hindi);
        Assert.That(result, Is.EqualTo(expectedUrdu));
    }

    [TestCase("\u0915\u093C", "\u0642")]       // क़ → ق
    [TestCase("\u0917\u093C", "\u063A")]       // ग़ → غ
    [TestCase("\u092B\u093C", "\u0641")]       // फ़ → ف
    [TestCase("\u091C\u093C", "\u0632")]       // ज़ → ز
    public void ShouldConvertNuktaConsonants(string hindi, string expectedUrdu)
    {
        var result = _transliterator.HindiToUrdu(hindi);
        Assert.That(result, Is.EqualTo(expectedUrdu));
    }

    [TestCase("\u0966", "\u06F0")]   // ० → ۰
    [TestCase("\u0967", "\u06F1")]   // १ → ۱
    [TestCase("\u096F", "\u06F9")]   // ९ → ۹
    public void ShouldConvertNumerals(string hindi, string expectedUrdu)
    {
        var result = _transliterator.HindiToUrdu(hindi);
        Assert.That(result, Is.EqualTo(expectedUrdu));
    }

    [TestCase("\u0916", "\u062E")]       // ख → خ
    [TestCase("\u0918", "\u063A")]       // घ → غ
    [TestCase("\u0925", "\u062A\u06BE")] // थ → تھ
    [TestCase("\u0927", "\u062F\u06BE")] // ध → دھ
    [TestCase("\u092D", "\u0628\u06BE")] // भ → بھ
    public void ShouldConvertAspiratedConsonants(string hindi, string expectedUrdu)
    {
        var result = _transliterator.HindiToUrdu(hindi);
        Assert.That(result, Is.EqualTo(expectedUrdu));
    }

    [Test]
    public void ShouldReturnEmptyForEmptyInput()
    {
        Assert.That(_transliterator.HindiToUrdu(""), Is.EqualTo(string.Empty));
    }
}
