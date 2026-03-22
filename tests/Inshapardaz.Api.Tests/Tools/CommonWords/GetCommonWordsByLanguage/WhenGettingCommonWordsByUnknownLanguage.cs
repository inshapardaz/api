using Bogus;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.CommonWords.GetCommonWordsByLanguage
{
    [TestFixture]
    public class WhenGettingCommonWordsByUnknownLanguage() : TestBase(Domain.Models.Role.Reader)
    {
        private HttpResponseMessage _response;
        private IEnumerable<string> _responsePayload; 
        private readonly string _language = new Faker().Random.String2(4);

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonWordBuilder.WithLanguage(_language).Build(4);
            
            _response = await Client.GetAsync($"/tools/-{_language}/words/list");
            var payload = await _response.Content.ReadAsStringAsync();
            _responsePayload = JsonConvert.DeserializeObject<IEnumerable<string>>(payload);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnNotFound() => _response.ShouldBeNotFound();
    }
}
