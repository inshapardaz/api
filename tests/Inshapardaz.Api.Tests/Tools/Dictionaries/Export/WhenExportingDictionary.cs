using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Dictionaries.Export
{
    [TestFixture("dsl", ".dsl")]
    [TestFixture("json", ".json")]
    [TestFixture("starDict", ".ifo")]
    public class WhenExportingDictionary(string format, string expectedExtension) : TestBase()
    {
        private HttpResponseMessage _response;
        private byte[] _zip;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dictionary = DictionaryBuilder.WithName("Export Test").WithHeadwords("kitab", "qalam").Build();
            _response = await Client.GetAsync($"/dictionaries/{dictionary.Id}/export?format={format}");
            _zip = await _response.Content.ReadAsByteArrayAsync();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnZip() => _response.Content.Headers.ContentType.MediaType.Should().Be("application/zip");

        [Test]
        public void ShouldContainDictionaryFile() =>
            DictionaryFileBuilder.ZipEntries(_zip).Should().Contain(e => e.EndsWith(expectedExtension));
    }

    [TestFixture]
    public class WhenExportingDictionaryAsDsl() : TestBase()
    {
        private string _dsl;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dictionary = DictionaryBuilder.WithName("Export Test").WithHeadwords("kitab", "qalam").Build();
            var zip = await (await Client.GetAsync($"/dictionaries/{dictionary.Id}/export?format=dsl")).Content.ReadAsByteArrayAsync();
            _dsl = DictionaryFileBuilder.ReadZipEntry(zip, DictionaryFileBuilder.ZipEntries(zip).Single(e => e.EndsWith(".dsl")));
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldContainHeadwordsAndMeanings()
        {
            _dsl.Should().Contain("Export Test");
            _dsl.Should().Contain("kitab").And.Contain("meaning of qalam");
        }
    }

    [TestFixture]
    public class WhenExportingDictionaryThatDoesNotExist() : TestBase()
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() => _response = await Client.GetAsync($"/dictionaries/{-RandomData.Number}/export");

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnNotFound() => _response.ShouldBeNotFound();
    }
}
