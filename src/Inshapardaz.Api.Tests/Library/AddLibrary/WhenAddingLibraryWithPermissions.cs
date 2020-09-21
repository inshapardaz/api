using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.AddLibrary
{
    [TestFixture]
    public class WhenAddingLibraryWithPermissions : TestBase
    {
        private LibraryView _library;
        private HttpResponseMessage _response;
        private LibraryAssert _assert;

        public WhenAddingLibraryWithPermissions()
            : base(Domain.Adapters.Permission.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _library = new LibraryView { Name = Random.Name, Language = Random.Locale, SupportsPeriodicals = Random.Bool };

            _response = await Client.PostObject($"/library", _library);
            _assert = LibraryAssert.FromResponse(_response, DatabaseConnection);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _assert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldHaveCreatedLibraryInDataStore()
        {
            _assert.ShouldHaveCreatedLibrary(DatabaseConnection);
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldHaveBooksLink()
        {
            _assert.ShouldHaveBooksLink();
        }

        [Test]
        public void ShouldHaveAuthorsLink()
        {
            _assert.ShouldHaveAuthorsLink();
        }

        [Test]
        public void ShouldHaveCategoriesLink()
        {
            _assert.ShouldHaveCategoriesLink();
        }

        [Test]
        public void ShouldHaveSeriesLink()
        {
            _assert.ShouldHaveSeriesLink();
        }

        [Test]
        public void ShouldHaveCorrectPeriodicalLink()
        {
            if (_periodicalsEnabled)
            {
                _assert.ShouldHavePeriodicalLink();
            }
            else
            {
                _assert.ShouldNotHavePeriodicalLink();
            }
        }

        [Test]
        public void ShouldHaveRecentLinks()
        {
            _assert.ShouldHaveRecentLinks();
        }

        [Test]
        public void ShouldHaveCreateCategoryLinks()
        {
            _assert.ShouldHaveCreateCategorylink()
                    .ShouldHaveCreatelink()
                    .ShouldHaveUpdatLlink()
                    .ShouldHaveDeleteLink();
        }
    }
}
