using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.GetBooks
{
    [TestFixture]
    public class WhenSearchingBooksByTitle : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;

        public WhenSearchingBooksByTitle()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            BookBuilder.WithLibrary(LibraryId).IsPublic().Build(30);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?pageNumber=2&pageSize=10&query=itle");

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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books");
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/books", 3);
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/books", 1);
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            var expectedItems = BookBuilder.Books.Where(b => b.Title.Contains("itle"))
                                              .OrderBy(a => a.Title).Skip(10).Take(10)
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
                            .ShouldHaveAddFavoriteLink()
                            .ShouldHavePublicImageLink();
            }
        }
    }
}
