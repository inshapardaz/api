using Inshapardaz.Adapters.Transliteration.Mappings;

namespace Inshapardaz.Adapters.Transliteration.Tests;

/// <summary>
/// Vocabulary-level tests for Roman to Urdu conversion.
/// Tests are based on common Urdu words and their standard romanised spellings.
/// 
/// IMPORTANT: Romanised Urdu is inherently ambiguous. The same Roman sound can map
/// to multiple Urdu characters:
///   - "s" → س (seen) or ص (swad) or ث (sa)
///   - "z" → ز (zay) or ض (dwad) or ظ (zoy)  
///   - "h" → ہ (he) or ح (ha) 
///   - "a" at word start → ا (alif) or ع (ain)
///   - "t" → ت (te) or ط (toy)
///
/// The converter maps to the MOST COMMON Urdu character for each sound.
/// Words with Arabic-origin characters (ع، ص، ض، ح، ط، ظ) require dictionary support.
/// 
/// Additionally, "kh" is ambiguous between خ (khay) and کھ (kaf + do-chashmi he),
/// "gh" between غ (ghain) and گھ (gaf + do-chashmi he), etc.
/// The converter defaults to the Arabic-origin letter for these digraphs.
/// </summary>
public class RomanToUrduVocabularyTests
{
    private readonly ScriptTransliterator _transliterator = ScriptTransliterator.Instance;

    // ===== Words that convert correctly =====

    [TestCase("pakistan", "\u067E\u0627\u06A9\u0633\u062A\u0627\u0646")] // پاکستان
    [TestCase("salam", "\u0633\u0644\u0627\u0645")]       // سلام
    [TestCase("khuda", "\u062E\u062F\u0627")]             // خدا
    [TestCase("shukria", "\u0634\u06A9\u0631\u06CC\u06C1")] // شکریہ
    [TestCase("ghar", "\u063A\u0627\u0631")]              // غار
    [TestCase("chai", "\u0686\u0627\u0626\u06D2")]        // چائے
    [TestCase("dil", "\u062F\u0644")]                     // دل
    public void PreviouslyVerifiedWords(string roman, string expectedUrdu)
    {
        var result = _transliterator.RomanToUrdu(roman);
        Assert.That(result, Is.EqualTo(expectedUrdu));
    }

    // ===== Basic vocabulary - simple unambiguous words =====

    [TestCase("bura", "\u0628\u0631\u0627")]                    // برا (bad)
    [TestCase("do", "\u062F\u0648")]                            // دو (two)
    [TestCase("teen", "\u062A\u06CC\u0646")]                    // تین (three)
    [TestCase("ruko", "\u0631\u06A9\u0648")]                    // رکو (stop)
    [TestCase("kon", "\u06A9\u0648\u0646")]                     // کون (who)
    [TestCase("shandaar", "\u0634\u0627\u0646\u062F\u0627\u0631")] // شاندار (fabulous)
    [TestCase("pasand", "\u067E\u0633\u0646\u062F")]            // پسند (like)
    [TestCase("banana", "\u0628\u0646\u0627\u0646\u0627")]      // بنانا (to make)
    public void SimpleUnambiguousWords(string roman, string expectedUrdu)
    {
        var result = _transliterator.RomanToUrdu(roman);
        Assert.That(result, Is.EqualTo(expectedUrdu));
    }

    // ===== Long vowels =====

    [TestCase("aasan", "\u0622\u0633\u0627\u0646")]             // آسان (easy)
    [TestCase("aana", "\u0622\u0646\u0627")]                    // آنا (to come)
    [TestCase("aaj", "\u0622\u062C")]                           // آج (today)
    [TestCase("saal", "\u0633\u0627\u0644")]                    // سال (year)
    [TestCase("saat", "\u0633\u0627\u062A")]                    // سات (seven)
    [TestCase("chaar", "\u0686\u0627\u0631")]                   // چار (four)
    [TestCase("paanch", "\u067E\u0627\u0646\u0686")]            // پانچ (five)
    [TestCase("door", "\u062F\u0648\u0631")]                    // دور (far)
    public void LongVowelWords(string roman, string expectedUrdu)
    {
        var result = _transliterator.RomanToUrdu(roman);
        Assert.That(result, Is.EqualTo(expectedUrdu));
    }

    // ===== Short vowels omitted =====

    [TestCase("mushkil", "\u0645\u0634\u06A9\u0644")]           // مشکل (hard)
    [TestCase("tum", "\u062A\u0645")]                           // تم (you)
    [TestCase("hum", "\u06C1\u0645")]                           // ہم (we)
    public void ShortUVowelOmitted(string roman, string expectedUrdu)
    {
        var result = _transliterator.RomanToUrdu(roman);
        Assert.That(result, Is.EqualTo(expectedUrdu));
    }

    // ===== Multi-word phrases =====

    [Test]
    public void MultiWordPhrase_PreservesSpaces()
    {
        var result = _transliterator.RomanToUrdu("mein pasand karta hoon");
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Does.Contain(" "));
        var words = result.Split(' ');
        Assert.That(words.Length, Is.EqualTo(4));
    }

    [Test]
    public void MultiWordPhrase_EachWordConverted()
    {
        var result = _transliterator.RomanToUrdu("mein istemal karta hoon");
        Assert.That(result, Is.Not.Empty);
        var words = result.Split(' ');
        Assert.That(words.Length, Is.EqualTo(4));
        // Each word should contain Urdu characters
        foreach (var word in words)
        {
            Assert.That(word.Any(c => c >= '\u0600' && c <= '\u06FF'), Is.True);
        }
    }

    // ===== Comprehensive vocabulary - all words produce valid Urdu output =====

    [TestCase("aasan")]
    [TestCase("mushkil")]
    [TestCase("acha")]
    [TestCase("bura")]
    [TestCase("nazdeek")]
    [TestCase("haan")]
    [TestCase("nae")]
    [TestCase("rakhna")]
    [TestCase("jana")]
    [TestCase("aana")]
    [TestCase("hansna")]
    [TestCase("banana")]
    [TestCase("chahna")]
    [TestCase("laina")]
    [TestCase("daikhna")]
    [TestCase("door")]
    [TestCase("chota")]
    [TestCase("barha")]
    [TestCase("khoobsoorat")]
    [TestCase("badsoorat")]
    [TestCase("mazaydar")]
    [TestCase("hafta")]
    [TestCase("saal")]
    [TestCase("aaj")]
    [TestCase("kal")]
    [TestCase("ghanta")]
    [TestCase("minet")]
    [TestCase("waqt")]
    [TestCase("peer")]
    [TestCase("mungal")]
    [TestCase("budh")]
    [TestCase("jumarat")]
    [TestCase("jumma")]
    [TestCase("itwar")]
    [TestCase("sifar")]
    [TestCase("aik")]
    [TestCase("do")]
    [TestCase("teen")]
    [TestCase("chaar")]
    [TestCase("paanch")]
    [TestCase("chhey")]
    [TestCase("saat")]
    [TestCase("aath")]
    [TestCase("no")]
    [TestCase("das")]
    [TestCase("mein")]
    [TestCase("tum")]
    [TestCase("wo")]
    [TestCase("hum")]
    [TestCase("kya")]
    [TestCase("kahan")]
    [TestCase("kon")]
    [TestCase("ruko")]
    [TestCase("ajeeb")]
    [TestCase("mutajassas")]
    [TestCase("khoobsurat")]
    [TestCase("aajiz")]
    [TestCase("badtameez")]
    [TestCase("wazeh")]
    [TestCase("shandaar")]
    [TestCase("hairat")]
    [TestCase("angaiz")]
    [TestCase("dilkash")]
    [TestCase("sasta")]
    [TestCase("deewana")]
    [TestCase("munfarid")]
    [TestCase("umeed")]
    public void AllVocabularyWordsProduceValidUrduOutput(string roman)
    {
        var result = _transliterator.RomanToUrdu(roman);

        Assert.That(string.IsNullOrEmpty(result), Is.False);

        Assert.That(result.Any(c => c >= '\u0600' && c <= '\u06FF'), Is.True);
    }

    // ===== Digits =====

    [Test]
    public void ShouldHandleDigitsInContext()
    {
        var result = _transliterator.RomanToUrdu("3 din");
        Assert.That(result, Does.StartWith("\u06F3")); // ۳
    }

    // ===== Converter behaviour documentation tests =====
    // These document current converter output for words where romanisation is ambiguous.
    // The "correct" Urdu would require dictionary-based disambiguation.

    [TestCase("deewana", "\u062F\u06CC\u0648\u0627\u0646\u0627")]   // دیوانا (converter: final a→alif; ideal: دیوانہ)
    [TestCase("umeed", "\u0627\u0645\u06CC\u062F")]                  // امید (hope)
    public void ConverterOutput_CommonWordsWithLongVowels(string roman, string expectedUrdu)
    {
        var result = _transliterator.RomanToUrdu(roman);
        Assert.That(result, Is.EqualTo(expectedUrdu));
    }

    [TestCase("nazdeek", "\u0646\u0627\u0632\u062F\u06CC\u06A9")]   // نازدیک (converter keeps 'a'; ideal: نزدیک)
    [TestCase("dilkash", "\u062F\u0644\u06A9\u0627\u0634")]         // دلکاش (converter keeps 'a'; ideal: دلکش)
    public void ConverterOutput_ShortAKeptAsAlif(string roman, string expectedUrdu)
    {
        // These words have short 'a' that in standard Urdu would be omitted,
        // but the converter keeps them as alif due to position rules.
        var result = _transliterator.RomanToUrdu(roman);
        Assert.That(result, Is.EqualTo(expectedUrdu));
    }
}
