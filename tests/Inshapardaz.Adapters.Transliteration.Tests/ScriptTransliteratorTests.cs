using Inshapardaz.Adapters.Transliteration.Mappings;

namespace Inshapardaz.Adapters.Transliteration.Tests;

/// <summary>
/// Tests for the generic Convert() API and edge cases.
/// </summary>
public class ScriptTransliteratorTests
{
    private readonly ScriptTransliterator _transliterator = ScriptTransliterator.Instance;

    [Test]
    public void ConvertSameScriptReturnsSameText()
    {
        const string text = "hello";
        Assert.That(_transliterator.Convert(text, Script.Roman, Script.Roman), Is.EqualTo(text));
    }

    [TestCase(Script.Roman, Script.UrduArabic)]
    [TestCase(Script.Roman, Script.HindiDevanagari)]
    [TestCase(Script.UrduArabic, Script.Roman)]
    [TestCase(Script.UrduArabic, Script.HindiDevanagari)]
    [TestCase(Script.HindiDevanagari, Script.Roman)]
    [TestCase(Script.HindiDevanagari, Script.UrduArabic)]
    public void AllConversionDirectionsAreSupported(Script from, Script to)
    {
        // Should not throw
        var result = _transliterator.Convert("test", from, to);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void ShouldHandleWhitespacePreservation()
    {
        var result = _transliterator.Convert("a b c", Script.Roman, Script.UrduArabic);
        Assert.That(result, Does.Contain(" "));
    }

    [Test]
    public void ShouldHandleMixedContent()
    {
        // English text that has no mapping entries passes through
        var result = _transliterator.UrduToRoman("Hello World");
        Assert.That(result, Is.EqualTo("Hello World"));
    }

    [Test]
    public void InstanceIsSingleton()
    {
        var a = ScriptTransliterator.Instance;
        var b = ScriptTransliterator.Instance;
        Assert.That(b, Is.SameAs(a));
    }

    [Test]
    public void RomanToUrduAndBackShouldBeReasonable()
    {
        // Round-trip won't be exact due to ambiguity, but should produce valid output
        var urdu = _transliterator.RomanToUrdu("pakistan");
        Assert.That(urdu, Is.Not.Empty);

        var backToRoman = _transliterator.UrduToRoman(urdu);
        Assert.That(backToRoman, Is.Not.Empty);
    }

    [Test]
    public void RomanToHindiAndBackShouldBeReasonable()
    {
        var hindi = _transliterator.RomanToHindi("bharat");
        Assert.That(hindi, Is.Not.Empty);

        var backToRoman = _transliterator.HindiToRoman(hindi);
        Assert.That(backToRoman, Is.Not.Empty);
    }
}
