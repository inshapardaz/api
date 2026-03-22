using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.UpdateLibrary
{
    [TestFixture]
    public class WhenUpdatingLibraryThatDoesNotExist() : TestBase(Role.Admin, createLibrary: false)
    {
        private HttpResponseMessage _response;
        private LibraryView _expectedLibrary;
        private LibraryAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expectedLibrary = new LibraryView { Name = RandomData.Name, Language = RandomData.Locale, SupportsPeriodicals = RandomData.Bool, FileStoreType = "Database" };

            _response = await Client.PutObject($"/libraries/{-RandomData.Number}", _expectedLibrary);
            var returnedView = await _response.GetContent<LibraryView>();
            _assert = Services.GetService<LibraryAssert>().ForResponse(_response).ForLibrary(returnedView.Id);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveCreatedResult() => _response.ShouldBeCreated();

        [Test]
        public void ShouldHaveLocationHeader() => _assert.ShouldHaveCorrectLocationHeader();

        [Test]
        public void ShouldHaveCreatedLibraryInDataStore() => _assert.ShouldHaveUpdatedLibrary();

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
