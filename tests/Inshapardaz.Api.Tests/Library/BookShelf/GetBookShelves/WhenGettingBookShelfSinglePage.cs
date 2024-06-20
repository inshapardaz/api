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
    public class WhenGettingBookShelfSinglePage : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookShelfView> _assert;

        public WhenGettingBookShelfSinglePage()
            :base(Domain.Models.Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            BookShelfBuilder.WithLibrary(LibraryId).WithBooks(3).WithoutImage().ForAccount(AccountId).Build(4);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/bookshelves");
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
        public void ShouldReturnExpectedBookShelf()
        {
            var expectedItems = BookShelfBuilder.BookShelves;
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                Services.GetService<BookShelfAssert>().ForView(actual).ForLibrary(LibraryId)
                      .ShouldBeSameAs(item)
                      .WithBookCount(3)
                      .WithEditableLinks()
                      .ShouldNotHaveImageLink();
            }
        }
    }
}
