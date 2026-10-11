using Bogus;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Spelling.CheckSpelling
{
    [TestFixture]
    public class WhenCheckingSpellingWithReplacementHistory() : TestBase(Role.Writer)
    {
        private readonly List<HttpResponseMessage> _replaceResponses = new();
        private SpellCheckResultView _result;
        private readonly string _language = new Faker().Random.String2(4);

        [OneTimeSetUp]
        public async Task Setup()
        {
            // "kitab" is the closest dictionary word, but writers have twice chosen "kutub" and once "katib".
            CommonWordBuilder.WithLanguage(_language).BuildWords("kitab", "kutub", "katib");

            foreach (var replacement in new[] { "kutub", "katib", "kutub" })
            {
                _replaceResponses.Add(await Client.PostObject($"/tools/{_language}/spellchecker/replace",
                    new SpellingReplacementView { Word = "Kitb", Replacement = replacement }));
            }

            var response = await Client.PostObject($"/tools/{_language}/spellchecker/check", new SpellCheckRequestView { Words = ["kitb"] });
            _result = (await response.GetContent<IEnumerable<SpellCheckResultView>>()).Single();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Services.GetService<ICommonWordsTestRepository>().DeleteSpellingReplacements(_language);
            Cleanup();
        }

        [Test]
        public void ShouldRecordReplacements() => _replaceResponses.Should().AllSatisfy(r => r.ShouldBeNoContent());

        [Test]
        public void ShouldRankMostChosenReplacementsFirst()
        {
            var suggestions = _result.Suggestions.ToList();
            suggestions[0].Word.Should().Be("kutub");
            suggestions[0].Priority.Should().Be(2);
            suggestions[1].Word.Should().Be("katib");
            suggestions[1].Priority.Should().Be(1);
        }

        [Test]
        public void ShouldStillIncludeDictionarySuggestionsOnce() =>
            _result.Suggestions.Select(s => s.Word).Should().ContainSingle(w => w == "kitab")
                .And.OnlyHaveUniqueItems();
    }
}
