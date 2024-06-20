using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.AddBookToBookShelf
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAddingBookToOtherUsersBookShelf : TestBase
    {
        private HttpResponseMessage _response;
        private BookDto _book;
        private BookShelfDto _bookShelf;

        public WhenAddingBookToOtherUsersBookShelf(Role role)
            :base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Build();
            _bookShelf = BookShelfBuilder.WithLibrary(Library.Id).ForAccount(account.Id).AsPublic().Build();
            _book = BookBuilder.WithLibrary(Library.Id).Build();
            
            var bookShelfBookView = new BookShelfBookView
            {
                BookId = _book.Id
            };

            _response = await Client.PostObject($"/libraries/{LibraryId}/bookshelves/{_bookShelf.Id}/books", bookShelfBookView);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResponse()
        {
            _response.ShouldBeForbidden();
        }

        [Test]
        public void ShouldNotAddBookToBookShelf()
        {
            var bookShelfbooks = BookShelfTestRepository.GetBookShelfBooks(_bookShelf.Id);
            bookShelfbooks.Should().NotContain(b => b.BookId == _book.Id);
        }
    }
}
