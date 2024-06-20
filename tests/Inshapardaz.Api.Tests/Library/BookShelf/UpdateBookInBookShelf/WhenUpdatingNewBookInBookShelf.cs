using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.UpdateBookInBookShelf
{
    [TestFixture]
    public class WhenUpdatingNewBookInBookShelf : TestBase
    {
        private HttpResponseMessage _response;
        private BookDto _book;
        private BookShelfDto _bookShelf;
        public WhenUpdatingNewBookInBookShelf()
            :base(Domain.Models.Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _bookShelf = BookShelfBuilder.WithLibrary(Library.Id).ForAccount(AccountId).AsPublic().Build();
            _book = BookBuilder.WithLibrary(Library.Id).Build();
            
            var bookShelfBookView = new BookShelfBookView
            {
                BookId = _book.Id,
                Index = 2
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/bookshelves/{_bookShelf.Id}/books/{_book.Id}", bookShelfBookView);
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
        public void ShouldAddBookToBookShelf()
        {
            var bookShelfbooks = BookShelfTestRepository.GetBookShelfBooks(_bookShelf.Id);
            bookShelfbooks.Should().Contain(b => b.BookId == _book.Id && b.Index == 2);
        }
    }
}
