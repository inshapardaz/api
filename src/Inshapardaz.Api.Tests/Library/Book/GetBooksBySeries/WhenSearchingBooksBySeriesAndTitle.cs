using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.GetBooksBySeries
{
    [TestFixture]
    public class WhenSearchingBooksBySeriesAndTitle
        : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private SeriesDto _series;
        private IEnumerable<BookDto> _seriesBooks;

        public WhenSearchingBooksBySeriesAndTitle()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _series = SeriesBuilder.WithLibrary(LibraryId).Build();
            _seriesBooks = BookBuilder.WithLibrary(LibraryId).WithSeries(_series).IsPublic().Build(25);
            SeriesBuilder.WithLibrary(LibraryId).WithBooks(3).Build();

            _response = await Client.GetAsync($"/library/{LibraryId}/books?pageNumber=2&pageSize=10&seriesid={_series.Id}&query=itle");
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
            _assert.ShouldHaveSelfLink($"/library/{LibraryId}/books", 2, 10, "seriesid", _series.Id.ToString());
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldNotHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/library/{LibraryId}/books", 3, 10, "query", "itle");
            _assert.ShouldHaveNextLink($"/library/{LibraryId}/books", 3, 10, "seriesid", _series.Id.ToString());
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/library/{LibraryId}/books", 1, 10, "query", "itle");
            _assert.ShouldHavePreviousLink($"/library/{LibraryId}/books", 1, 10, "seriesid", _series.Id.ToString());
        }

        [Test]
        public void ShouldReturnCorrectPage()
        {
            _assert.ShouldHavePage(2)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(_seriesBooks.Count())
                   .ShouldHaveItems(10);
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            var expectedItems = _seriesBooks.Where(b => b.Title.Contains("itle"))
                                            .OrderBy(a => a.Title)
                                            .Skip(10)
                                            .Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatch(item, DatabaseConnection)
                            .InLibrary(LibraryId)
                            .ShouldHaveCorrectLinks()
                            .ShouldHaveSeriesLink()
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
