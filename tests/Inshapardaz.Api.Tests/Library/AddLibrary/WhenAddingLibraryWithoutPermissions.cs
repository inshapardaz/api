using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.AddLibrary
{
    [TestFixture(Role.Reader)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenAddingLibraryWithoutPermissions(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var library = new LibraryView { OwnerEmail = RandomData.Email, Name = RandomData.Name, Language = RandomData.Locale, SupportsPeriodicals = RandomData.Bool, FileStoreType = "Database" };

            _response = await Client.PostObject($"/libraries", library);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveForbiddenResult() => _response.ShouldBeForbidden();
    }
}
