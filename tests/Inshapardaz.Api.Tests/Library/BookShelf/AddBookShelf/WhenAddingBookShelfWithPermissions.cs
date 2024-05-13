using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.AddBookShelf
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenAddingBookShelfWithPermissions
        : TestBase
    {
        private BookShelfAssert _assert;
        private HttpResponseMessage _response;

        public WhenAddingBookShelfWithPermissions(Role role) : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var bookShelf = new BookShelfView { Name = RandomData.Name, Description = RandomData.Text, IsPublic = true };

            _response = await Client.PostObject($"/libraries/{LibraryId}/bookshelves", bookShelf);

            _assert = BookShelfAssert.WithResponse(_response).InLibrary(LibraryId);
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
        public void ShouldSaveTheSeries()
        {
            _assert.ShouldHaveSavedBookShelf(DatabaseConnection, AccountId);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                         .ShouldHaveBooksLink()
                         .ShouldHaveUpdateLink()
                         .ShouldHaveDeleteLink()
                         .ShouldHaveImageUpdateLink();
        }
    }
}
