using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.CommonWords.GetCommonWordsByLanguage
{
    [TestFixture]
    public class WhenGettingCommonWordsByLanguage : TestBase
    {
        private HttpResponseMessage _response;
        private IEnumerable<string> _responsePayload; 
        private readonly string _language = new Faker().Random.String2(4);

        public WhenGettingCommonWordsByLanguage()
            :base(Domain.Models.Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonWordBuilder.WithLanguage(_language).Build(12);
            
            _response = await Client.GetAsync($"/tools/{_language}/words/list");
            var payload = await _response.Content.ReadAsStringAsync();
            _responsePayload = JsonConvert.DeserializeObject<IEnumerable<string>>(payload);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldReturnExpectedData()
        {
            var expectedItems = CommonWordBuilder.Words.OrderBy(a => a.Word).Select(x => x.Word);
            _responsePayload.Should().Equal(expectedItems);
        }
    }
}
