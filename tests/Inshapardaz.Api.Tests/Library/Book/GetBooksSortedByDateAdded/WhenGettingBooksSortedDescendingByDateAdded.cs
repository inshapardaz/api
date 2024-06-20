using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.GetBooksSortedByDateAdded
{
    [TestFixture]
    public class WhenGettingBooksSortedDescendingByDateAdded
        : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private IEnumerable<BookDto> _books;

        public WhenGettingBooksSortedDescendingByDateAdded()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _books = BookBuilder.WithLibrary(LibraryId).IsPublic().Build(12);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?pageNumber=1&pageSize=10&sortby=DateCreated&sortDirection=Descending");
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books",
                new KeyValuePair<string, string>("sortby", "DateCreated"),
                new KeyValuePair<string, string>("sortDirection", "Descending"));
        }

        [Test]
        public void ShouldNotHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/books", 2, 10,
                new KeyValuePair<string, string>("sortby", "DateCreated"),
                new KeyValuePair<string, string>("sortDirection", "Descending"));
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldReturnCorrectPage()
        {
            _assert.ShouldHavePage(1)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(_books.Count())
                   .ShouldHaveItems(10);
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            var expectedItems = _books.OrderByDescending(a => a.DateAdded).Take(10).ToArray();

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
