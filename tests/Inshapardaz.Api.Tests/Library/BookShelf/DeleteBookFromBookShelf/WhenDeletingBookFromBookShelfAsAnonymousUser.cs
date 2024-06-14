using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.DeleteBookFromBookShelf
{
    [TestFixture]
    public class WhenDeletingBookFromBookShelfAsAnonymousUser : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Build();
            var bookShelf = BookShelfBuilder.WithLibrary(Library.Id).ForAccount(account.Id).WithBooks(2).AsPublic().Build();
            var book = BookShelfBuilder.Books.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/bookshelves/{bookShelf.Id}/books/{book.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
