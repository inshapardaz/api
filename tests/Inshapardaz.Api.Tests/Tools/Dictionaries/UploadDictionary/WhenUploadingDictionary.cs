using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Dictionaries;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Dictionaries.UploadDictionary
{
    [TestFixture]
    public class WhenUploadingDslDictionary() : TestBase(Role.Reader, isDictionaryWriter: true)
    {
        private HttpResponseMessage _response;
        private DictionaryView _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dsl = DictionaryFileBuilder.Dsl("Test DSL", ("كتاب", "book"), ("قلم", "pen"));
            _response = await Client.PostContent("/dictionaries/upload", dsl, "text/plain", "test.dsl");
            _view = await _response.GetContent<DictionaryView>();
            DictionaryBuilder.Track(_view.Id);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeCreated() => _response.ShouldBeCreated();

        [Test]
        public void ShouldTakeDetailsFromFile()
        {
            _view.Name.Should().Be("Test DSL");
            _view.Format.Should().Be(DictionaryFormat.Dsl);
            _view.SourceLanguage.Should().Be("Urdu");
            _view.TargetLanguage.Should().Be("English");
            _view.EntryCount.Should().Be(2);
            _view.OwnerId.Should().Be(AccountId);
        }

        [Test]
        public void ShouldStoreArticlesWithNormalisedHeadwords()
        {
            var articles = Services.GetService<IDictionaryTestRepository>().GetArticles(_view.Id).ToList();
            articles.Select(a => a.Headword).Should().BeEquivalentTo("كتاب", "قلم");
            articles.Single(a => a.Headword == "كتاب").HeadwordNormalised.Should().Be("کتاب");
            articles.Single(a => a.Headword == "قلم").RawDefinition.Should().Contain("pen");
        }
    }

    // Consecutive headword lines in DSL are alternate spellings of one entry.
    [TestFixture]
    public class WhenUploadingDslDictionaryWithAlternateHeadwords() : TestBase(Role.Admin)
    {
        private List<DictionaryArticleDto> _articles;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dsl = DictionaryFileBuilder.Dsl("Alternates", ("colour\ncolor", "a hue"), ("grey\ngray", "a colour"), ("red", "a colour"));
            var response = await Client.PostContent("/dictionaries/upload", dsl, "text/plain", "test.dsl");
            var view = await response.GetContent<DictionaryView>();
            DictionaryBuilder.Track(view.Id);
            _articles = Services.GetService<IDictionaryTestRepository>().GetArticles(view.Id).ToList();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldCreateOneArticlePerEntry() => _articles.Select(a => a.Headword).Should().Equal("colour", "grey", "red");

        [Test]
        public void ShouldKeepOnlyEachEntrysOwnAlternates()
        {
            _articles[0].Alternates.Should().Be("[\"color\"]");
            _articles[1].Alternates.Should().Be("[\"gray\"]");
            _articles[2].Alternates.Should().BeNull();
        }
    }

    [TestFixture]
    public class WhenUploadingStarDictDictionaryWithOverrides() : TestBase(Role.Admin)
    {
        private HttpResponseMessage _response;
        private DictionaryView _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var zip = DictionaryFileBuilder.StarDictZip("Star", ("apple", "a fruit"), ("bank", "edge of a river"), ("cat", "an animal"));
            using var content = new MultipartFormDataContent();
            var file = new ByteArrayContent(zip);
            file.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
            content.Add(file, "file", "star.zip");
            content.Add(new StringContent("English Words", Encoding.UTF8), "name");
            content.Add(new StringContent("en", Encoding.UTF8), "sourceLanguage");

            _response = await Client.PostAsync("/dictionaries/upload", content);
            _view = await _response.GetContent<DictionaryView>();
            DictionaryBuilder.Track(_view.Id);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeCreated() => _response.ShouldBeCreated();

        [Test]
        public void ShouldUseOverridesOverFileMetadata()
        {
            _view.Name.Should().Be("English Words");
            _view.SourceLanguage.Should().Be("en");
            _view.Format.Should().Be(DictionaryFormat.StarDict);
        }

        [Test]
        public void ShouldImportAllArticles()
        {
            _view.EntryCount.Should().Be(3);
            var articles = Services.GetService<IDictionaryTestRepository>().GetArticles(_view.Id).ToList();
            articles.Single(a => a.Headword == "bank").RawDefinition.Should().Be("edge of a river");
        }
    }

    [TestFixture]
    public class WhenUploadingUnrecognisedDictionaryFile() : TestBase(Role.Admin)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() =>
            _response = await Client.PostContent("/dictionaries/upload", Encoding.UTF8.GetBytes("just some notes"), "text/plain", "notes.txt");

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeBadRequest() => _response.ShouldBeBadRequest();
    }

    [TestFixture]
    public class WhenUploadingDictionaryWithoutPermission() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() =>
            _response = await Client.PostContent("/dictionaries/upload", DictionaryFileBuilder.Dsl("x", ("a", "b")), "text/plain", "test.dsl");

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeForbidden() => _response.ShouldBeForbidden();
    }
}
