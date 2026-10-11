using Bogus;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Tools;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Spelling.CheckSpelling
{
    [TestFixture]
    public class WhenCheckingSpelling() : TestBase()
    {
        private HttpResponseMessage _response;
        private List<SpellCheckResultView> _results;
        private readonly string _language = new Faker().Random.String2(4);

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonWordBuilder.WithLanguage(_language).BuildWords("kitab", "qalam", "dawat", "کتاب");

            _response = await Client.PostObject($"/tools/{_language}/spellchecker/check", new SpellCheckRequestView
            {
                Words = ["kitab", "kitb", "qlam", "123", "zzzzzzzzzz", "كتاب", "kitab"]
            });
            _results = (await _response.GetContent<IEnumerable<SpellCheckResultView>>()).ToList();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnEachDistinctWordOnce() =>
            _results.Select(r => r.Word).Should().Equal("kitab", "kitb", "qlam", "123", "zzzzzzzzzz", "كتاب");

        [Test]
        public void ShouldFindKnownWord() => Result("kitab").Found.Should().BeTrue();

        [Test]
        public void ShouldTreatNumbersAsCorrect() => Result("123").Found.Should().BeTrue();

        [Test]
        public void ShouldMatchArabicKafAgainstUrduKeheh() => Result("كتاب").Found.Should().BeTrue();

        [Test]
        public void ShouldSuggestClosestWords()
        {
            Result("kitb").Found.Should().BeFalse();
            Result("kitb").Suggestions.First().Word.Should().Be("kitab");
            Result("qlam").Suggestions.First().Word.Should().Be("qalam");
        }

        [Test]
        public void ShouldNotSuggestUnrelatedWords()
        {
            Result("zzzzzzzzzz").Found.Should().BeFalse();
            Result("zzzzzzzzzz").Suggestions.Should().BeEmpty();
        }

        private SpellCheckResultView Result(string word) => _results.Single(r => r.Word == word);
    }
}
