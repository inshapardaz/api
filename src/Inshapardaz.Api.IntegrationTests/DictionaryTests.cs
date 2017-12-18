using System.Net.Http;
using Inshapardaz.Api.View;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Inshapardaz.Api.IntegrationTests
{
    [TestFixture]
    public class DictionaryTests : IntegrationTestBase
    {
        private HttpResponseMessage _response;
        private EntryView _view;

        [OneTimeSetUp]
        public void Setup()
        {
            _response = GetClient().GetAsync("/api/dictionaries").Result;
            _view = JsonConvert.DeserializeObject<EntryView>(_response.Content.ReadAsStringAsync().Result);
        }
    }
}
