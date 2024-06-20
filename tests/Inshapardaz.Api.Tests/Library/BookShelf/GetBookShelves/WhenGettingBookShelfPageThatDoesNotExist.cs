using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.GetBookShelves
{
    [TestFixture]
    public class WhenGettingBookShelfPageThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookShelfView> _assert;

        public WhenGettingBookShelfPageThatDoesNotExist()
            :base(Role.Reader)
        {        
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            BookShelfBuilder.WithLibrary(LibraryId).WithBooks(3).ForAccount(AccountId).Build(20);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/bookshelves?pageNumber=100&pageSize=10");
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/bookshelves");
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
        public void ShouldNotHavePreviousLink()
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
