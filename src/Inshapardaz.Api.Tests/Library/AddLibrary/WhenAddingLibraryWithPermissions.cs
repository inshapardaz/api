using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.AddLibrary
{
    [TestFixture]
    public class WhenAddingLibraryWithPermissions : TestBase
    {
        private LibraryView _library;
        private HttpResponseMessage _response;
        private LibraryView _returnedView;
        private LibraryAssert _assert;

        public WhenAddingLibraryWithPermissions()
            : base(Role.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _library = new LibraryView { 
                OwnerEmail = RandomData.Email, 
                Name = RandomData.Name, 
                Language = RandomData.Locale, 
                SupportsPeriodicals = _periodicalsEnabled,
                DatabaseConnection = RandomData.String,
                FileStoreType = RandomData.String,
                FileStoreSource = RandomData.String 
            };

            _response = await Client.PostObject($"/libraries", _library);
            _returnedView = await _response.GetContent<LibraryView>();
            _assert = LibraryAssert.FromResponse(_response, DatabaseConnection);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
            DatabaseConnection.DeleteLibrary(_returnedView.Id);
            DatabaseConnection.DeleteAccountByEmail(_library.OwnerEmail);
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
            _assert.ShouldHaveCreatedLibraryWithConfiguration(DatabaseConnection);
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
                .WithName(_library.Name)
                .ShouldBeInvited()
                .ShouldHaveInvitationExpiring(DateTime.Today.AddDays(7))
                .ShouldNotBeVerified()
                .ShouldBeInRole(Role.LibraryAdmin, _returnedView.Id)
                .InLibrary(_returnedView.Id);
        }

        [Test]
        public void ShouldHaveSentEmailToAdministrator()
        {
            var dbAccount = DatabaseConnection.GetAccountByEmail(_library.OwnerEmail);
            SmtpClient.AssertEmailSentTo(_library.OwnerEmail)
                .WithSubject($"Welcome to {_library.Name}")
                .WithBodyContainting($"Hi, Welcome to your library {_library.Name}")
                .WithBodyContainting(dbAccount.InvitationCode);
        }
    }
}
