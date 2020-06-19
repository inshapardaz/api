using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.GetBooksByCategory
{
    [TestFixture]
    public class WhenSearchingBooksByCategoryAndTitle
        : LibraryTest<Functions.Library.Books.GetBooks>
    {
        private OkObjectResult _response;
        private PagingAssert<BookView> _assert;
        private CategoriesDataBuilder _categoryBuilder;
        private CategoryDto _category;
        private IEnumerable<BookDto> _categoryBooks;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categoryBuilder = Container.GetService<CategoriesDataBuilder>();
            var _bookBuilder = Container.GetService<BooksDataBuilder>();
            _category = _categoryBuilder.WithLibrary(LibraryId).Build();
            _categoryBooks = _bookBuilder.WithLibrary(LibraryId).WithCategory(_category).IsPublic().Build(25);
            _categoryBuilder.WithLibrary(LibraryId).WithBooks(3).Build();

            var request = new RequestBuilder()
                          .WithQueryParameter("pageNumber", 2)
                          .WithQueryParameter("pageSize", 10)
                          .WithQueryParameter("query", "itle")
                          .WithQueryParameter("categoryid", _category.Id)
                          .Build();

            _response = (OkObjectResult)await handler.Run(request, LibraryId, AuthenticationBuilder.ReaderClaim, CancellationToken.None);

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
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/books", 2, 10, "query", "itle");
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/books", 2, 10, "categoryid", _category.Id.ToString());
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/api/library/{LibraryId}/books", 3, 10, "query", "itle");
            _assert.ShouldHaveNextLink($"/api/library/{LibraryId}/books", 3, 10, "categoryid", _category.Id.ToString());
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/api/library/{LibraryId}/books", 1, 10, "query", "itle");
            _assert.ShouldHavePreviousLink($"/api/library/{LibraryId}/books", 1, 10, "categoryid", _category.Id.ToString());
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
