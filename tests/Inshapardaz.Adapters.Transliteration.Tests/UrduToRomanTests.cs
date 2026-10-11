using Inshapardaz.Adapters.Transliteration.Mappings;

namespace Inshapardaz.Adapters.Transliteration.Tests;

public class UrduToRomanTests
{
    private readonly ScriptTransliterator _transliterator = ScriptTransliterator.Instance;

    [TestCase("\u0633\u0644\u0627\u0645", "slam")]       // سلام → slam
    [TestCase("\u067E\u0627\u06A9\u0633\u062A\u0627\u0646", "pakstan")] // پاکستان (no zer = no 'i')
    [TestCase("\u062F\u0644", "dl")]                     // دل → dl
    [TestCase("\u0634", "sh")]                           // ش
    [TestCase("\u0686", "ch")]                           // چ
    [TestCase("\u062E", "kh")]                           // خ
    [TestCase("\u063A", "gh")]                           // غ
    public void ShouldConvertUrduCharacters(string urdu, string expectedRoman)
    {
        var result = _transliterator.UrduToRoman(urdu);
        Assert.That(result, Is.EqualTo(expectedRoman));
    }

    [TestCase("\u06F0", "0")]
    [TestCase("\u06F1", "1")]
    [TestCase("\u06F9", "9")]
    [TestCase("\u06F1\u06F2\u06F3", "123")]
    public void ShouldConvertNumerals(string urdu, string expectedRoman)
    {
        var result = _transliterator.UrduToRoman(urdu);
        Assert.That(result, Is.EqualTo(expectedRoman));
    }

    [TestCase("\u06D4", ".")]    // ۔
    [TestCase("\u060C", ",")]    // ،
    [TestCase("\u061F", "?")]    // ؟
    public void ShouldConvertPunctuation(string urdu, string expectedRoman)
    {
        var result = _transliterator.UrduToRoman(urdu);
        Assert.That(result, Is.EqualTo(expectedRoman));
    }

    [Test]
    public void ShouldReturnEmptyForEmptyInput()
    {
        Assert.That(_transliterator.UrduToRoman(""), Is.EqualTo(string.Empty));
    }
}
