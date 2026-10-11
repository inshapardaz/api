using Inshapardaz.Adapters.Transliteration.Mappings;

namespace Inshapardaz.Adapters.Transliteration.Tests;

public class RomanToUrduTests
{
    private readonly ScriptTransliterator _transliterator = ScriptTransliterator.Instance;

    [TestCase("salam", "\u0633\u0644\u0627\u0645")]           // salam → سلام
    [TestCase("pakistan", "\u067E\u0627\u06A9\u0633\u062A\u0627\u0646")] // pakistan → پاکستان
    [TestCase("khuda", "\u062E\u062F\u0627")]                 // khuda → خدا
    [TestCase("shukria", "\u0634\u06A9\u0631\u06CC\u06C1")]   // shukria → شکریہ
    [TestCase("dil", "\u062F\u0644")]                         // dil → دل
    [TestCase("ghar", "\u063A\u0627\u0631")]                  // ghar → غار
    [TestCase("raat", "\u0631\u0627\u062A")]                  // raat → رات
    [TestCase("chai", "\u0686\u0627\u0626\u06D2")]            // chai → چائے
    public void ShouldConvertCommonWords(string roman, string expectedUrdu)
    {
        var result = _transliterator.RomanToUrdu(roman);
        Assert.That(result, Is.EqualTo(expectedUrdu));
    }

    [TestCase("kh", "\u062E")]   // خ
    [TestCase("gh", "\u063A")]   // غ
    [TestCase("ch", "\u0686")]   // چ
    [TestCase("sh", "\u0634")]   // ش
    [TestCase("th", "\u062B")]   // ث
    [TestCase("tt", "\u0679")]   // ٹ
    [TestCase("dd", "\u0688")]   // ڈ
    [TestCase("rr", "\u0691")]   // ڑ
    public void ShouldHandleDigraphs(string roman, string expectedUrdu)
    {
        var result = _transliterator.RomanToUrdu(roman);
        Assert.That(result, Is.EqualTo(expectedUrdu));
    }

    [TestCase("1", "\u06F1")]
    [TestCase("2", "\u06F2")]
    [TestCase("123", "\u06F1\u06F2\u06F3")]
    public void ShouldConvertNumerals(string roman, string expectedUrdu)
    {
        var result = _transliterator.RomanToUrdu(roman);
        Assert.That(result, Is.EqualTo(expectedUrdu));
    }

    [Test]
    public void ShouldReturnEmptyForEmptyInput()
    {
        Assert.That(_transliterator.RomanToUrdu(""), Is.EqualTo(string.Empty));
        Assert.That(_transliterator.RomanToUrdu(null!), Is.EqualTo(string.Empty));
    }

    [Test]
    public void ShouldPreserveSpaces()
    {
        var result = _transliterator.RomanToUrdu("salam dost");
        Assert.That(result, Does.Contain(" "));
    }

    [Test]
    public void WordInitialVowelShouldGetAlif()
    {
        // Word-initial "a" → alif
        var result = _transliterator.RomanToUrdu("ab");
        Assert.That(result, Does.StartWith("\u0627")); // starts with ا
    }

    [Test]
    public void LongVowelsShouldProduceFullLetters()
    {
        // "aa" at start → آ (alif madda)
        var result = _transliterator.RomanToUrdu("aam");
        Assert.That(result, Does.StartWith("\u0622")); // آ
    }
}
