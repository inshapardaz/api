using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.UpdateBookShelf
{
    [TestFixture]
    public class WhenUpdatingBookShelfThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private BookShelfView _bookShelf;
        private BookShelfAssert _assert;

        public WhenUpdatingBookShelfThatDoesNotExist()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _bookShelf = new BookShelfView { Name = RandomData.Name, Description = RandomData.Text, IsPublic = false };

            _response = await Client.PutObject($"/libraries/{LibraryId}/bookshelves/{-RandomData.Number}", _bookShelf);
            _assert = Services.GetService<BookShelfAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
        public void ShouldHaveCreatedTheBookSelf()
        {
            _assert.ShouldHaveSavedBookShelf(AccountId);
        }
    }
}
