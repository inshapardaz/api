using System.Text;
using Bogus;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.CommonWords.ImportCommonWords
{
    [TestFixture(Role.Admin, false)]
    [TestFixture(Role.Reader, true)]
    public class WhenImportingCommonWords(Role role, bool isDictionaryWriter) : TestBase(role, isDictionaryWriter: isDictionaryWriter)
    {
        private HttpResponseMessage _response;
        private WordImportResultView _result;
        private readonly string _language = new Faker().Random.String2(4);

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonWordBuilder.WithLanguage(_language).BuildWords("کتاب");

            // "كتاب" normalises to the existing "کتاب"; "qalam" is repeated; blanks are ignored.
            _response = await Client.PostObject($"/tools/{_language}/words/bulk", new WordImportRequestView
            {
                Words = ["qalam", "dawat", "كتاب", "qalam", " ", "kaghaz"]
            });
            _result = await _response.GetContent<WordImportResultView>();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Services.GetService<ICommonWordsTestRepository>().DeleteWordsForLanguage(_language);
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReportCounts()
        {
            _result.Added.Should().Be(3);
            _result.Skipped.Should().Be(3);
        }

        [Test]
        public void ShouldHaveSavedNewWordsOnly() =>
            Services.GetService<ICommonWordsTestRepository>().GetWordsForLanguage(_language)
                .Should().BeEquivalentTo("کتاب", "qalam", "dawat", "kaghaz");
    }

    [TestFixture(Role.Writer)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenImportingCommonWordsWithoutPermission(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private readonly string _language = new Faker().Random.String2(4);

        [OneTimeSetUp]
        public async Task Setup() =>
            _response = await Client.PostObject($"/tools/{_language}/words/bulk", new WordImportRequestView { Words = ["qalam"] });

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeForbidden() => _response.ShouldBeForbidden();

        [Test]
        public void ShouldNotHaveSavedWords() =>
            Services.GetService<ICommonWordsTestRepository>().GetWordsForLanguage(_language).Should().BeEmpty();
    }

    [TestFixture]
    public class WhenUploadingHunspellDictionary() : TestBase(Role.Admin)
    {
        private HttpResponseMessage _response;
        private WordImportResultView _result;
        private readonly string _language = new Faker().Random.String2(4);

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dic = "3\nkitab/S\nqalam/SM po:noun\n# comment\ndawat\n";
            _response = await Client.PostContent($"/tools/{_language}/words/upload", Encoding.UTF8.GetBytes(dic), "text/plain", "ur.dic");
            _result = await _response.GetContent<WordImportResultView>();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Services.GetService<ICommonWordsTestRepository>().DeleteWordsForLanguage(_language);
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldImportWordsWithoutAffixFlags()
        {
            _result.Added.Should().Be(3);
            Services.GetService<ICommonWordsTestRepository>().GetWordsForLanguage(_language)
                .Should().BeEquivalentTo("kitab", "qalam", "dawat");
        }
    }
}
