using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.DeleteLibrary
{
    [TestFixture]
    public class WhenDeletingNonExistingLibrary : TestBase
    {
        private HttpResponseMessage _response;

        public WhenDeletingNonExistingLibrary()
            : base(Domain.Adapters.Permission.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.DeleteAsync($"/library/{-Random.Number}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNoContentResult()
        {
            _response.ShouldBeNoContent();
        }
    }
}
