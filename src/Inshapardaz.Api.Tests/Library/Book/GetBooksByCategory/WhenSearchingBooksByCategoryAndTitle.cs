using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.GetBooksByCategory
{
    [TestFixture]
    public class WhenSearchingBooksByCategoryAndTitle
        : TestBase

    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private CategoryDto _category;
        private IEnumerable<BookDto> _categoryBooks;

        public WhenSearchingBooksByCategoryAndTitle() : base(Domain.Adapters.Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _category = CategoryBuilder.WithLibrary(LibraryId).Build();
            _categoryBooks = BookBuilder.WithLibrary(LibraryId).WithCategory(_category).IsPublic().Build(25);
            CategoryBuilder.WithLibrary(LibraryId).WithBooks(3).Build();

            _response = await Client.GetAsync($"/library/{LibraryId}/books?pageNumber=2&pageSize=10&categoryid={_category.Id}&query=itle");

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
            _assert.ShouldHaveSelfLink($"/library/{LibraryId}/books", 2, 10, "query", "itle");
            _assert.ShouldHaveSelfLink($"/library/{LibraryId}/books", 2, 10, "categoryid", _category.Id.ToString());
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/library/{LibraryId}/books", 3, 10, "query", "itle");
            _assert.ShouldHaveNextLink($"/library/{LibraryId}/books", 3, 10, "categoryid", _category.Id.ToString());
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/library/{LibraryId}/books", 1, 10, "query", "itle");
            _assert.ShouldHavePreviousLink($"/library/{LibraryId}/books", 1, 10, "categoryid", _category.Id.ToString());
        }

        [Test]
        public void ShouldReturnCorrectPage()
        {
            _assert.ShouldHavePage(2)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(_categoryBooks.Count())
                   .ShouldHaveItems(10);
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            var expectedItems = _categoryBooks.Where(b => b.Title.Contains("itle"))
                                              .OrderBy(a => a.Title)
                                              .Skip(10)
                                              .Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatch(item, DatabaseConnection)
                            .InLibrary(LibraryId)
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
