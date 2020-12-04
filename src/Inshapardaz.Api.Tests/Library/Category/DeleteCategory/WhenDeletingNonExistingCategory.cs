using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.DeleteCategory
{
    [TestFixture]
    public class WhenDeletingNonExistingCategory : TestBase
    {
        private HttpResponseMessage _response;

        public WhenDeletingNonExistingCategory() : base(Role.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.DeleteAsync($"/library/{LibraryId}/categories/{-Random.Number}");
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
