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

namespace Inshapardaz.Functions.Tests.Library.Book.GetBooksBySeries
{
    [TestFixture]
    public class WhenGettingBooksBySeriesPageThatDoesNotExist
        : LibraryTest<Functions.Library.Books.GetBooks>
    {
        private OkObjectResult _response;
        private PagingAssert<BookView> _assert;
        private SeriesDataBuilder _categoryBuilder;
        private SeriesDto _series;
        private IEnumerable<BookDto> _SeriesBooks;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categoryBuilder = Container.GetService<SeriesDataBuilder>();
            var _bookBuilder = Container.GetService<BooksDataBuilder>();
            _series = _categoryBuilder.WithLibrary(LibraryId).Build();
            _SeriesBooks = _bookBuilder.WithLibrary(LibraryId).WithSeries(_series).IsPublic().Build(25);
            _categoryBuilder.WithLibrary(LibraryId).WithBooks(3).Build();

            var request = new RequestBuilder()
                          .WithQueryParameter("pageNumber", 4)
                          .WithQueryParameter("pageSize", 10)
                          .WithQueryParameter("seriesid", _series.Id)
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
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/books", 4, 10, "seriesid", _series.Id.ToString());
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
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
        public void ShouldReturnCorrectPage()
        {
            _assert.ShouldHavePage(4)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(_SeriesBooks.Count())
                   .ShouldHaveItems(0);
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            _assert.ShouldHaveNoData();
        }
    }
}
