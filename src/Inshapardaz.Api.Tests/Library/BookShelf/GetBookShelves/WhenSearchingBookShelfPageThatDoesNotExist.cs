using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.GetBookShelves
{
    [TestFixture]
    public class WhenSearchingBookShelfPageThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookShelfView> _assert;
        private const string searchTerm = "SearchBKS";
        public WhenSearchingBookShelfPageThatDoesNotExist() 
            : base(Domain.Models.Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            BookShelfBuilder.WithLibrary(LibraryId).WithBooks(3).WithNamePattern(searchTerm).ForAccount(AccountId).Build(20);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/bookshelves?query={searchTerm}&pageNumber=100&pageSize=10");
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
        public void ShouldNotHaveNextLink()
        {
            _assert.ShouldNotHaveNextLink();
        }

        [Test]
        public void ShouldNotHavepreviousLinks()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnNoData()
        {
            _assert.ShouldHaveNoData();
        }
    }
}
