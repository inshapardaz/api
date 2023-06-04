using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.GetBookShelves
{
    [TestFixture]
    public class WhenSearchingBookShelfSinglePage : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookShelfView> _assert;
        private const string searchTerm = "SearchBKS";

        public WhenSearchingBookShelfSinglePage()
            :base(Domain.Models.Role.Reader)
        {
        }


        [OneTimeSetUp]
        public async Task Setup()
        {
            BookShelfBuilder.WithLibrary(LibraryId).WithBooks(3).WithNamePattern(searchTerm).ForAccount(AccountId).Build(5);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/bookshelves?query={searchTerm}&pageNumber=1&pageSize=10");
            _assert = new PagingAssert<BookShelfView>(_response);
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
            _assert.ShouldNotHaveNextLink();
        }

        [Test]
        public void ShouldNotHavepreviousLinks()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnExpectedSeries()
        {
            var expectedItems = BookShelfBuilder.BookShelves.Where(a => a.Name.Contains(searchTerm));
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatch(item)
                      .InLibrary(LibraryId)
                      .WithBookCount(3)
                      .WithEditableLinks()
                      .ShouldHavePublicImageLink();
            }
        }
    }
}
