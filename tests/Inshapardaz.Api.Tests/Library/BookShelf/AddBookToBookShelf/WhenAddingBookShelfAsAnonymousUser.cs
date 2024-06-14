using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.AddBookToBookShelf
{
    [TestFixture]
    public class WhenAddingBookToBookShelfAsAnonymousUser : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Build();
            var bookShelf = BookShelfBuilder.WithLibrary(Library.Id).ForAccount(account.Id).AsPublic().Build();
            var book = BookBuilder.WithLibrary(Library.Id).Build();

            var bookShelfBookView = new BookShelfBookView
            {
                BookId = book.Id
            };
            _response = await Client.PostObject($"/libraries/{LibraryId}/bookshelves/{bookShelf.Id}/books", bookShelfBookView);
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
