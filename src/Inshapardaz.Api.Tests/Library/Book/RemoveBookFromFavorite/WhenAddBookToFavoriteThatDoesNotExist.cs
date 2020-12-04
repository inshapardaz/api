using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.RemoveBookFromFavorite
{
    [TestFixture]
    public class WhenAddBookToFavoriteThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private int _bookId = -Random.Number;

        public WhenAddBookToFavoriteThatDoesNotExist()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.DeleteAsync($"/library/{LibraryId}/favorites/books/{_bookId}");
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
            BookAssert.ShouldNotBeInFavorites(_bookId, AccountId, DatabaseConnection);
        }
    }
}
