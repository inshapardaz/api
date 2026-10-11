using Bogus;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models.Transliteration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Transliteration
{
    internal static class UniqueRoman
    {
        // Letters only, so the rule-based engine would never produce the dictionary spelling.
        public static string Next() => new Faker().Random.String2(12, "abcdefghijklmnopqrstuvwxyz");
    }

    [TestFixture]
    public class WhenTransliteratingWithDictionary() : TestBase()
    {
        private HttpResponseMessage _response;
        private TransliterationResultView _result;
        private TransliterationEntryDto _word;
        private readonly string _wordRoman = UniqueRoman.Next();
        private readonly string _phraseRoman = $"{UniqueRoman.Next()} o {UniqueRoman.Next()}";

        [OneTimeSetUp]
        public async Task Setup()
        {
            var repository = Services.GetService<ITransliterationTestRepository>();
            _word = repository.AddEntry("ur", _wordRoman, "خدا");
            repository.AddEntry("ur", _phraseRoman, "السلام علیکم");

            _response = await Client.PostObject("/tools/ur/transliterate", new TransliterateRequestView
            {
                Text = $"{_wordRoman.ToUpperInvariant()}, {_phraseRoman} dil",
                From = TransliterationScript.Roman,
                To = TransliterationScript.UrduArabic,
                Details = true
            });
            _result = await _response.GetContent<TransliterationResultView>();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Services.GetService<ITransliterationTestRepository>().DeleteEntries("ur", [_wordRoman, _phraseRoman]);
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldUseDictionaryForWordsAndPhrases() => _result.Result.Should().StartWith("خدا, السلام علیکم ");

        [Test]
        public void ShouldUseRulesForUnknownWords() => _result.Result.Should().NotEndWith("dil").And.NotBe(string.Empty);

        [Test]
        public void ShouldExplainEachWord()
        {
            var words = _result.Words.ToList();
            words[0].Should().BeEquivalentTo(new TransliteratedWordView { Input = _wordRoman.ToUpperInvariant(), Output = "خدا", Source = "dictionary", EntryId = _word.Id });
            words[1].Source.Should().Be("punctuation");
            words.Last().Source.Should().Be("rules");
        }
    }

    [TestFixture]
    public class WhenTransliteratingIntoAnotherLanguagesScript() : TestBase()
    {
        private TransliterationResultView _result;
        private readonly string _roman = UniqueRoman.Next();

        [OneTimeSetUp]
        public async Task Setup()
        {
            Services.GetService<ITransliterationTestRepository>().AddEntry("ur", _roman, "خدا");

            // The Urdu dictionary holds Urdu spellings, so it can't help produce Devanagari.
            var response = await Client.PostObject("/tools/ur/transliterate", new TransliterateRequestView
            {
                Text = _roman,
                From = TransliterationScript.Roman,
                To = TransliterationScript.HindiDevanagari
            });
            _result = await response.GetContent<TransliterationResultView>();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Services.GetService<ITransliterationTestRepository>().DeleteEntries("ur", [_roman]);
            Cleanup();
        }

        [Test]
        public void ShouldNotUseDictionary() => _result.Result.Should().NotBe("خدا");

        [Test]
        public void ShouldNotReturnBreakdownUnlessAsked() => _result.Words.Should().BeNull();
    }

    [TestFixture]
    public class WhenTransliteratingForUnsupportedLanguage() : TestBase()
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() =>
            _response = await Client.PostObject("/tools/fr/transliterate", new TransliterateRequestView { Text = "salam", To = TransliterationScript.UrduArabic });

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeBadRequest() => _response.ShouldBeBadRequest();
    }
}
