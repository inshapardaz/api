using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.DeleteLibrary
{
    [TestFixture]
    public class WhenDeletingLibraryWithPermission : TestBase
    {
        private HttpResponseMessage _response;

        public WhenDeletingLibraryWithPermission()
            : base(Permission.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.DeleteAsync($"/library/{LibraryId}");
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

        [Test]
        public void ShouldHaveDeletedLibrary()
        {
            LibraryAssert.ShouldHaveDeletedLibrary(LibraryId, DatabaseConnection);
        }
    }
}
