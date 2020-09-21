using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.DeleteAuthor
{
    [TestFixture]
    public class WhenDeletingNonExistingAuthor : TestBase
    {
        private HttpResponseMessage _response;

        public WhenDeletingNonExistingAuthor()
            : base(Domain.Adapters.Permission.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.DeleteAsync($"/library/{LibraryId}/authors/{-Random.Number}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }
    }
}
