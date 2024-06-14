using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.UpdateBookInBookShelf
{
    [TestFixture]
    public class WhenUpdatingBookInBookShelfUnauthorised : TestBase
    {
        private HttpResponseMessage _response;
        private BookDto _book;
        private BookShelfDto _bookShelf;
        private int ExpectedIndex = RandomData.Number;


        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Build();
            _bookShelf = BookShelfBuilder.WithLibrary(Library.Id).ForAccount(account.Id).AsPublic().WithBooks(2).Build();
            _book = BookShelfBuilder.Books.PickRandom();
            
            var bookShelfBookView = new BookShelfBookView
            {
                BookId = _book.Id,
                Index = ExpectedIndex
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/bookshelves/{_bookShelf.Id}/books/{_book.Id}", bookShelfBookView);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorisedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
