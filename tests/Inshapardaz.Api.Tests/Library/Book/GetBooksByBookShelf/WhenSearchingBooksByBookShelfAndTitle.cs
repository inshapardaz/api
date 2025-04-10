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
    public class WhenSearchingBooksByBookShelfAndTitle
        : TestBase

    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private BookShelfDto _bookShelf;
        private IEnumerable<BookDto> _bookShelfBooks;

        public WhenSearchingBooksByBookShelfAndTitle() : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _bookShelf = BookShelfBuilder.WithLibrary(LibraryId).ForAccount(AccountId).Build();
            _bookShelfBooks = BookBuilder.WithLibrary(LibraryId).InBookShelf(_bookShelf.Id).IsPublic().Build(25);
            CategoryBuilder.WithLibrary(LibraryId).WithBooks(3).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?pageNumber=2&pageSize=10&bookShelfId={_bookShelf.Id}&query=itle");

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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books", 2, 10,
                new KeyValuePair<string, string>("query", "itle"));
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books", 2, 10,
                new KeyValuePair<string, string>("bookShelfId", _bookShelf.Id.ToString()));
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/books", 3, 10, new KeyValuePair<string, string>("query", "itle"));
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/books", 3, 10, new KeyValuePair<string, string>("bookShelfId", _bookShelf.Id.ToString()));
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/books", 1, 10, new KeyValuePair<string, string>("query", "itle"));
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/books", 1, 10, new KeyValuePair<string, string>("bookShelfId", _bookShelf.Id.ToString()));
        }

        [Test]
        public void ShouldReturnCorrectPage()
        {
            _assert.ShouldHavePage(2)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(_bookShelfBooks.Count())
                   .ShouldHaveItems(10);
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            var expectedItems = _bookShelfBooks.Where(b => b.Title.Contains("itle"))
                                              .OrderBy(a => a.Title)
                                              .Skip(10)
                                              .Take(10)
                                              .ToArray();
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                Services.GetService<BookAssert>().ForView(actual).ForLibrary(LibraryId)
                            .ShouldBeSameAs(expected)
                            .ShouldHaveCorrectLinks()
                            .ShouldNotHaveEditLinks()
                            .ShouldNotHaveImageUpdateLink()
                            .ShouldNotHaveCreateChaptersLink()
                            .ShouldNotHaveAddContentLink()
                            .ShouldHaveChaptersLink()
                            .ShouldHavePublicImageLink();
            }
        }
    }
}
