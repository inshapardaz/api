using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.DeleteLibrary
{
    [TestFixture(Permission.Reader)]
    [TestFixture(Permission.Writer)]
    [TestFixture(Permission.LibraryAdmin)]
    public class WhenDeletingLibraryWithoutPermissions : TestBase
    {
        private HttpResponseMessage _response;

        public WhenDeletingLibraryWithoutPermissions(Permission permission)
            : base(permission)
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
        public void ShouldHaveForbiddenResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}
