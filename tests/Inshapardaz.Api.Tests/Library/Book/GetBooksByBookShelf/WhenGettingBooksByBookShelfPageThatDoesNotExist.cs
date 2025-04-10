using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.GetBooksByBookShelf
{
    [TestFixture]
    public class WhenGettingBooksByBookShelfPageThatDoesNotExist
        : TestBase

    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private BookShelfDto _bookShelf;
        private IEnumerable<BookDto> _bookShelfBooks;

        public WhenGettingBooksByBookShelfPageThatDoesNotExist()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _bookShelf = BookShelfBuilder.WithLibrary(LibraryId).ForAccount(AccountId).Build();
            _bookShelfBooks = BookBuilder.WithLibrary(LibraryId).InBookShelf(_bookShelf.Id).IsPublic().Build(25);
            CategoryBuilder.WithLibrary(LibraryId).WithBooks(3).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?pageNumber=4&pageSize=10&bookShelfId={_bookShelf.Id}");

            _assert = Services.GetService<PagingAssert<BookView>>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books", 4, 10,
                new KeyValuePair<string, string>("bookShelfId", _bookShelf.Id.ToString()));
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldNotHaveNextLink()
        {
            _assert.ShouldNotHaveNextLink();
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnCorrectPage()
        {
            _assert.ShouldHavePage(4)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(_bookShelfBooks.Count())
                   .ShouldHaveItems(0);
        }

        [Test]
        public void ShouldReturnNoBooks()
        {
            _assert.ShouldHaveNoData();
        }
    }
}
