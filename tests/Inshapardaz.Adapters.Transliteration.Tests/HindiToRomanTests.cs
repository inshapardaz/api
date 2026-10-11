using Inshapardaz.Adapters.Transliteration.Mappings;

namespace Inshapardaz.Adapters.Transliteration.Tests;

public class HindiToRomanTests
{
    private readonly ScriptTransliterator _transliterator = ScriptTransliterator.Instance;

    [TestCase("\u0915", "k")]       // क
    [TestCase("\u0916", "kh")]      // ख
    [TestCase("\u0917", "g")]       // ग
    [TestCase("\u0918", "gh")]      // घ
    [TestCase("\u091A", "ch")]      // च
    [TestCase("\u091C", "j")]       // ज
    [TestCase("\u091F", "tt")]      // ट
    [TestCase("\u0924", "t")]       // त
    [TestCase("\u0925", "th")]      // थ
    [TestCase("\u0926", "d")]       // द
    [TestCase("\u0927", "dh")]      // ध
    [TestCase("\u0928", "n")]       // न
    [TestCase("\u092A", "p")]       // प
    [TestCase("\u092B", "ph")]      // फ
    [TestCase("\u092C", "b")]       // ब
    [TestCase("\u092D", "bh")]      // भ
    [TestCase("\u092E", "m")]       // म
    [TestCase("\u0930", "r")]       // र
    [TestCase("\u0932", "l")]       // ल
    [TestCase("\u0936", "sh")]      // श
    [TestCase("\u0938", "s")]       // स
    [TestCase("\u0939", "h")]       // ह
    public void ShouldConvertSingleConsonants(string hindi, string expectedRoman)
    {
        var result = _transliterator.HindiToRoman(hindi);
        Assert.That(result, Is.EqualTo(expectedRoman));
    }

    [TestCase("\u0905", "a")]       // अ
    [TestCase("\u0906", "aa")]      // आ
    [TestCase("\u0907", "i")]       // इ
    [TestCase("\u0908", "ee")]      // ई
    [TestCase("\u0909", "u")]       // उ
    [TestCase("\u090A", "oo")]      // ऊ
    [TestCase("\u090F", "e")]       // ए
    [TestCase("\u0910", "ai")]      // ऐ
    [TestCase("\u0913", "o")]       // ओ
    [TestCase("\u0914", "au")]      // औ
    public void ShouldConvertIndependentVowels(string hindi, string expectedRoman)
    {
        var result = _transliterator.HindiToRoman(hindi);
        Assert.That(result, Is.EqualTo(expectedRoman));
    }

    [TestCase("\u0966", "0")]
    [TestCase("\u0967", "1")]
    [TestCase("\u096F", "9")]
    public void ShouldConvertNumerals(string hindi, string expectedRoman)
    {
        var result = _transliterator.HindiToRoman(hindi);
        Assert.That(result, Is.EqualTo(expectedRoman));
    }

    [TestCase("\u0915\u094D\u0930", "kr")]   // क्र (virama suppresses inherent a)
    public void ShouldHandleVirama(string hindi, string expectedRoman)
    {
        var result = _transliterator.HindiToRoman(hindi);
        Assert.That(result, Is.EqualTo(expectedRoman));
    }

    [Test]
    public void ShouldReturnEmptyForEmptyInput()
    {
        Assert.That(_transliterator.HindiToRoman(""), Is.EqualTo(string.Empty));
    }
}
