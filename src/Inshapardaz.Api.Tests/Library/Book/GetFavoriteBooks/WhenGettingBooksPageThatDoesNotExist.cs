using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.GetFavoriteBooks
{
    [TestFixture]
    public class WhenGettingBooksPageThatDoesNotExist
            : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;

        public WhenGettingBooksPageThatDoesNotExist()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            BookBuilder.WithLibrary(LibraryId).IsPublic().AddToFavorites(AccountId).Build(5);

            _response = await Client.GetAsync($"/library/{LibraryId}/books?pageNumber=6&pageSize=10&favorite=true");
            _assert = new PagingAssert<BookView>(_response, Library);
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
            _assert.ShouldHaveSelfLink($"/library/{LibraryId}/books");
        }

        [Test]
        public void ShouldHaveNextLink()
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
            _assert.ShouldHavePage(6)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(BookBuilder.Books.Count())
                   .ShouldHaveItems(0);
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            _assert.ShouldHaveNoData();
        }
    }
}
