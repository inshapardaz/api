using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.UpdateLibrary
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    public class WhenUpdatingLibraryWithPermission : TestBase
    {
        private HttpResponseMessage _response;

        private LibraryView _expectedLibrary;
        private LibraryAssert _assert;

        public WhenUpdatingLibraryWithPermission(Permission permission)
            : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expectedLibrary = new LibraryView { Name = Random.Name, Language = Random.Locale, SupportsPeriodicals = Random.Bool };

            _response = await Client.PutObject($"/library/{LibraryId}", _expectedLibrary);
            _assert = LibraryAssert.FromResponse(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveReturnedUpdatedTheLibrary()
        {
            _assert.ShouldBeSameAs(_expectedLibrary);
        }

        [Test]
        public void ShouldHaveUpdatedLibrary()
        {
            _assert.ShouldHaveUpdatedLibrary(DatabaseConnection);
        }
    }
}
