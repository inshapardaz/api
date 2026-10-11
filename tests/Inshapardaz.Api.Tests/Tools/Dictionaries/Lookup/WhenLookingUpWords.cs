using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Tools;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Dictionaries.Lookup
{
    [TestFixture]
    public class WhenLookingUpWords() : TestBase()
    {
        private DictionaryDto _first;
        private DictionaryDto _second;
        private List<DictionaryArticleView> _all;
        private List<DictionaryArticleView> _filtered;
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            // A headword made unique per run so other tests' data can't match.
            var word = $"کتاب{Guid.NewGuid():N}";
            _first = DictionaryBuilder.WithName("First").WithHeadwords(word, "qalam").Build();
            _second = DictionaryBuilder.WithName("Second").WithHeadwords(word).Build();

            // Typed with Arabic kaf; the dictionaries hold the Urdu keheh form.
            var query = word.Replace('ک', 'ك');
            _response = await Client.GetAsync($"/dictionaries/lookup?word={Uri.EscapeDataString(query)}");
            _all = (await _response.GetContent<IEnumerable<DictionaryArticleView>>()).ToList();
            _filtered = (await (await Client.GetAsync($"/dictionaries/lookup?word={Uri.EscapeDataString(query)}&dictionaryIds={_second.Id}"))
                .GetContent<IEnumerable<DictionaryArticleView>>()).ToList();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldFindWordInEveryDictionary()
        {
            _all.Select(a => a.DictionaryId).Should().BeEquivalentTo([_first.Id, _second.Id]);
            _all.Select(a => a.DictionaryName).Should().BeEquivalentTo("First", "Second");
        }

        [Test]
        public void ShouldRestrictToChosenDictionaries() => _filtered.Select(a => a.DictionaryId).Should().Equal(_second.Id);

        [Test]
        public void ShouldReturnMeanings() => _all.First().Senses.Single().Meanings.Single().Definition.Should().StartWith("meaning of");
    }

    [TestFixture]
    public class WhenSuggestingHeadwords() : TestBase()
    {
        private HeadwordSuggestionsView _result;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var prefix = $"zq{Guid.NewGuid():N}"[..12];
            var dictionary = DictionaryBuilder.WithHeadwords($"{prefix}a", $"{prefix}b", "other").Build();
            var response = await Client.GetAsync($"/dictionaries/suggest?prefix={prefix.ToUpperInvariant()}&dictionaryIds={dictionary.Id}");
            response.ShouldBeOk();
            _result = await response.GetContent<HeadwordSuggestionsView>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnHeadwordsWithPrefix() => _result.Suggestions.Should().HaveCount(2).And.OnlyContain(s => !s.Equals("other"));
    }

    [TestFixture]
    public class WhenLookingUpWithoutWord() : TestBase()
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() => _response = await Client.GetAsync("/dictionaries/lookup?word=");

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeBadRequest() => _response.ShouldBeBadRequest();
    }
}
