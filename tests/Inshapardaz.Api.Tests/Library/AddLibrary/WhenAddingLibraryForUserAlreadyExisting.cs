using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.AddLibrary
{
    [TestFixture]
    public class WhenAddingLibraryForUserAlreadyExisting : TestBase
    {
        private LibraryView _library;
        private HttpResponseMessage _response;
        private LibraryView _returnedView;
        private LibraryAssert _assert;

        public WhenAddingLibraryForUserAlreadyExisting()
            : base(Role.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _library = new LibraryView { 
                OwnerEmail = Account.Email, 
                Name = RandomData.Name, 
                Language = RandomData.Locale, 
                SupportsPeriodicals = _periodicalsEnabled, 
                FileStoreType = "Database" };

            _response = await Client.PostObject($"/libraries", _library);
            _returnedView = await _response.GetContent<LibraryView>();
            _assert = LibraryAssert.FromResponse(_response, DatabaseConnection);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
            DatabaseConnection.DeleteLibrary(_returnedView.Id);
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
        public void ShouldHaveEditLinks()
        {
            _assert.ShouldHaveCreateCategorylink()
                    .ShouldHaveUpdateLink()
                    .ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveCreatedAccount()
        {
            AccountAssert.AssertAccountExistsWithEmail(_library.OwnerEmail)
                .ShouldBeVerified()
                .ShouldBeInRole(Role.LibraryAdmin, _returnedView.Id)
                .InLibrary(_returnedView.Id);
        }

        [Test]
        public void ShouldNotSentEmailToAdministrator()
        {
            SmtpClient.AssertNoEmailSent();
        }
    }
}
