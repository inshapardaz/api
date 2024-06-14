using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.UpdateLibrary
{
    [TestFixture]
    public class WhenUpdatingLibraryThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private LibraryView _expectedLibrary;
        private LibraryAssert _assert;

        public WhenUpdatingLibraryThatDoesNotExist()
            : base(Role.Admin, createLibrary: false)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expectedLibrary = new LibraryView { Name = RandomData.Name, Language = RandomData.Locale, SupportsPeriodicals = RandomData.Bool, FileStoreType = "Database" };

            _response = await Client.PutObject($"/libraries/{-RandomData.Number}", _expectedLibrary);
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
            _assert.ShouldHaveUpdatedLibrary(DatabaseConnection);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                    .ShouldHaveBooksLink()
                    .ShouldHaveUpdateLink()
                    .ShouldHaveDeleteLink();
        }
    }
}
