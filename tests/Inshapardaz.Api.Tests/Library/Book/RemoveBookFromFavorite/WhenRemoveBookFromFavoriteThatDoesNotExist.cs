using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.RemoveBookFromFavorite
{
    [TestFixture]
    public class WhenRemoveBookFromFavoriteThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private BookAssert _assert;
        private int _bookId = -RandomData.Number;

        public WhenRemoveBookFromFavoriteThatDoesNotExist()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/favorites/books/{_bookId}");
            _assert = Services.GetService<BookAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
        public void ShouldNotBeInFavorites()
        {
            _assert.ShouldNotBeInFavorites(_bookId, AccountId);
        }
    }
}
