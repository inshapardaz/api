using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.DeleteBookFromBookShelf
{
    [TestFixture]
    public class WhenDeletingBookFromBookShelf : TestBase
    {
        private HttpResponseMessage _response;
        private BookDto _book;
        private BookShelfDto _bookShelf;
        public WhenDeletingBookFromBookShelf()
            :base(Domain.Models.Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _bookShelf = BookShelfBuilder.WithLibrary(Library.Id).ForAccount(AccountId).WithBooks(2).AsPublic().Build();
            _book = BookShelfBuilder.Books.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/bookshelves/{_bookShelf.Id}/books/{_book.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNoContentResult()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldNotContainBookToBookShelf()
        {
            var bookShelfbooks = BookShelfTestRepository.GetBookShelfBooks(_bookShelf.Id);
            bookShelfbooks.Should().NotContain(b => b.BookId == _book.Id);
        }
    }
}
