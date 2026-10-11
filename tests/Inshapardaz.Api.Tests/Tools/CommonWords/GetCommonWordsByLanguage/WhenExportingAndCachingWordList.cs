using System.Net;
using Bogus;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.CommonWords.GetCommonWordsByLanguage
{
    [TestFixture]
    public class WhenGettingWordListWithMatchingETag() : TestBase()
    {
        private HttpResponseMessage _first;
        private HttpResponseMessage _second;
        private readonly string _language = new Faker().Random.String2(4);

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonWordBuilder.WithLanguage(_language).BuildWords("kitab", "qalam");

            _first = await Client.GetAsync($"/tools/{_language}/words/list");

            var request = new HttpRequestMessage(HttpMethod.Get, $"/tools/{_language}/words/list");
            request.Headers.IfNoneMatch.Add(_first.Headers.ETag);
            _second = await Client.SendAsync(request);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnETag()
        {
            _first.ShouldBeOk();
            _first.Headers.ETag.Should().NotBeNull();
        }

        [Test]
        public void ShouldReturnNotModifiedForSameVersion() => _second.StatusCode.Should().Be(HttpStatusCode.NotModified);
    }

    [TestFixture]
    public class WhenExportingWords() : TestBase()
    {
        private HttpResponseMessage _response;
        private string _content;
        private readonly string _language = new Faker().Random.String2(4);

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonWordBuilder.WithLanguage(_language).BuildWords("qalam", "kitab");

            _response = await Client.GetAsync($"/tools/{_language}/words/export");
            _content = await _response.Content.ReadAsStringAsync();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnOneWordPerLineInOrder() => _content.Should().Be("kitab\nqalam\n");

        [Test]
        public void ShouldReturnTextFile() => _response.Content.Headers.ContentType.MediaType.Should().Be("text/plain");
    }
}
