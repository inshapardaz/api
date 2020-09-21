using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.GetSeriesById
{
    [TestFixture]
    public class WhenGettingSeriesByIdThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;

        public WhenGettingSeriesByIdThatDoesNotExist()
            : base(Domain.Adapters.Permission.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.GetAsync($"/library/{LibraryId}/series/{-Random.Number}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNotFoundResult()
        {
            _response.ShouldBeNotFound();
        }
    }
}
