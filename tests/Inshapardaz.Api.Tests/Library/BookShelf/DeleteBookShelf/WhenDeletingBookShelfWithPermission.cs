using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.DeleteBookShelf
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingBookShelfWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private BookShelfAssert _assert;
        private BookShelfDto _expected;
        private string _filePath;

        public WhenDeletingBookShelfWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var bookShelf = BookShelfBuilder.WithLibrary(LibraryId).WithBooks(3).ForAccount(AccountId).Build(4);
            _expected = bookShelf.PickRandom();
            _filePath = FileTestRepository.GetFileById(_expected.ImageId.Value)?.FilePath;

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/bookshelves/{_expected.Id}");
            _assert = Services.GetService<BookShelfAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedBookShelf()
        {
            _assert.ShouldHaveDeletedBookShelf(_expected.Id);
        }

        [Test]
        public void ShouldHaveDeletedTheBookShelfImage()
        {
            _assert.ShouldHaveDeletedBookShelfImage(_expected.Id, _expected.ImageId.Value, _filePath);
        }

        [Test]
        public void ShouldNotDeleteBooks()
        {
            var books = BookShelfBuilder.BookShelvesBookList[_expected.Id];
            foreach (var book in books)
            {
                var b = BookTestRepository.GetBookById(book.Id);
                b.SeriesId.Should().BeNull();
                b.SeriesIndex.Should().BeNull();
            }
        }
    }
}
