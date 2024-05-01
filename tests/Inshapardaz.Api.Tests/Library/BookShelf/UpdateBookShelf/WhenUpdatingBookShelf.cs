using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.UpdateBookShelf
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenUpdatingBookShelf : TestBase
    {
        private HttpResponseMessage _response;
        private BookShelfView _expected;
        private BookShelfAssert _assert;

        public WhenUpdatingBookShelf(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var bookshelf = BookShelfBuilder.WithLibrary(LibraryId).ForAccount(Account.Id).AsPublic().Build();

            _expected = new BookShelfView { Name = RandomData.Name, Description = RandomData.Text, IsPublic = false };

            _response = await Client.PutObject($"/libraries/{LibraryId}/bookshelves/{bookshelf.Id}", _expected);
            _assert = BookShelfAssert.WithResponse(_response).InLibrary(LibraryId);
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
        public void ShouldHaveUpdatedTheBookShelf()
        {
            _assert.ShouldHaveSavedBookShelf(DatabaseConnection, Account.Id);
        }
    }
}
