using Inshapardaz.Adapters.Transliteration.Mappings;

namespace Inshapardaz.Adapters.Transliteration.Tests;

public class RomanToHindiTests
{
    private readonly ScriptTransliterator _transliterator = ScriptTransliterator.Instance;

    [TestCase("k", "\u0915")]       // क
    [TestCase("kh", "\u0916")]      // ख
    [TestCase("g", "\u0917")]       // ग
    [TestCase("gh", "\u0918")]      // घ
    [TestCase("ch", "\u091A")]      // च
    [TestCase("j", "\u091C")]       // ज
    [TestCase("t", "\u0924")]       // त
    [TestCase("th", "\u0925")]      // थ
    [TestCase("d", "\u0926")]       // द
    [TestCase("dh", "\u0927")]      // ध
    [TestCase("n", "\u0928")]       // न
    [TestCase("p", "\u092A")]       // प
    [TestCase("b", "\u092C")]       // ब
    [TestCase("bh", "\u092D")]      // भ
    [TestCase("m", "\u092E")]       // म
    [TestCase("r", "\u0930")]       // र
    [TestCase("l", "\u0932")]       // ल
    [TestCase("sh", "\u0936")]      // श
    [TestCase("s", "\u0938")]       // स
    [TestCase("h", "\u0939")]       // ह
    public void ShouldConvertSingleConsonants(string roman, string expectedHindi)
    {
        var result = _transliterator.RomanToHindi(roman);
        Assert.That(result, Is.EqualTo(expectedHindi));
    }

    [TestCase("tt", "\u091F")]      // ट
    [TestCase("dd", "\u0921")]      // ड
    [TestCase("ph", "\u092B")]      // फ
    public void ShouldConvertRetroflexAndAspirates(string roman, string expectedHindi)
    {
        var result = _transliterator.RomanToHindi(roman);
        Assert.That(result, Is.EqualTo(expectedHindi));
    }

    [TestCase("0", "\u0966")]
    [TestCase("1", "\u0967")]
    [TestCase("9", "\u096F")]
    [TestCase("123", "\u0967\u0968\u0969")]
    public void ShouldConvertNumerals(string roman, string expectedHindi)
    {
        var result = _transliterator.RomanToHindi(roman);
        Assert.That(result, Is.EqualTo(expectedHindi));
    }

    [Test]
    public void ShouldReturnEmptyForEmptyInput()
    {
        Assert.That(_transliterator.RomanToHindi(""), Is.EqualTo(string.Empty));
        Assert.That(_transliterator.RomanToHindi(null!), Is.EqualTo(string.Empty));
    }

    [Test]
    public void ShouldPreserveSpaces()
    {
        var result = _transliterator.RomanToHindi("ek do");
        Assert.That(result, Does.Contain(" "));
    }
}
