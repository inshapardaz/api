using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.UpdateLibrary
{
    [TestFixture(Role.Reader)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingLibraryWithoutPermission(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var library = new LibraryView { Name = RandomData.Name, Language = RandomData.Locale, SupportsPeriodicals = RandomData.Bool, FileStoreType = "Database" };

            _response = await Client.PutObject($"/libraries/{LibraryId}", library);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveForbiddenResult() => _response.ShouldBeForbidden();
    }
}
