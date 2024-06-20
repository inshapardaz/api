using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.GetBookShelves
{
    [TestFixture]
    public class WhenSearchingBookShelfInMiddle : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookShelfView> _assert;
        private const string searchTerm = "SearchBKS";

        public WhenSearchingBookShelfInMiddle()
            :base(Domain.Models.Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            BookShelfBuilder.WithLibrary(LibraryId).WithBooks(3).WithNamePattern(searchTerm).ForAccount(AccountId).Build(50);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/bookshelves?query={searchTerm}&pageNumber=3&pageSize=10");
            _assert = Services.GetService<PagingAssert<BookShelfView>>().ForResponse(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/bookshelves", new KeyValuePair<string, string>("query", searchTerm));
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            _assert.ShouldHaveCreateLink($"/libraries/{LibraryId}/bookshelves");
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/bookshelves", 4, 10, new KeyValuePair<string, string>("query", searchTerm));
        }

        [Test]
        public void ShouldHavePreviousLinks()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/bookshelves", 2, 10, new KeyValuePair<string, string>("query", searchTerm));
        }

        [Test]
        public void ShouldReturnExpectedSeries()
        {
            var expectedItems = BookShelfBuilder.BookShelves.Where(a => a.Name.Contains(searchTerm))
                .OrderBy(a => a.Name)
                .Skip(2 * 10).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                Services.GetService<BookShelfAssert>().ForView(actual).ForLibrary(LibraryId)
                      .ShouldBeSameAs(item)
                      .WithBookCount(3)
                      .WithEditableLinks()
                      .ShouldHavePublicImageLink();
            }
        }
    }
}
