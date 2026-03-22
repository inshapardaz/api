using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.DeleteLibrary
{
    [TestFixture(Role.Reader)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenDeletingLibraryWithoutPermissions(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() => _response = await Client.DeleteAsync($"/libraries/{LibraryId}");

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveForbiddenResult() => _response.ShouldBeForbidden();
    }
}
