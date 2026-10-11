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
    // The spelling index is cached per language; adding a word through the API must refresh it.
    [TestFixture]
    public class WhenCheckingSpellingAfterAddingWord() : TestBase(Role.Writer)
    {
        private SpellCheckResultView _before;
        private SpellCheckResultView _after;
        private readonly string _language = new Faker().Random.String2(4);

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonWordBuilder.WithLanguage(_language).BuildWords("kitab");

            _before = await Check("qalam");
            await Client.PostObject($"/tools/{_language}/words", new CommonWordView { Word = "qalam" });
            _after = await Check("qalam");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Services.GetService<ICommonWordsTestRepository>().DeleteWordsForLanguage(_language);
            Cleanup();
        }

        [Test]
        public void ShouldNotFindWordBeforeItIsAdded() => _before.Found.Should().BeFalse();

        [Test]
        public void ShouldFindWordOnceAdded() => _after.Found.Should().BeTrue();

        private async Task<SpellCheckResultView> Check(string word)
        {
            var response = await Client.PostObject($"/tools/{_language}/spellchecker/check", new SpellCheckRequestView { Words = [word] });
            response.ShouldBeOk();
            return (await response.GetContent<IEnumerable<SpellCheckResultView>>()).Single();
        }
    }
}
