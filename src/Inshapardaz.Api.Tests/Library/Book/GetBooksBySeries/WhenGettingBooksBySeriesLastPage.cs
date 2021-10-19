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
    public class WhenGettingBooksBySeriesLastPage
        : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private SeriesDto _series;
        private IEnumerable<BookDto> _SeriesBooks;

        public WhenGettingBooksBySeriesLastPage()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _series = SeriesBuilder.WithLibrary(LibraryId).Build();
            _SeriesBooks = BookBuilder.WithLibrary(LibraryId).WithSeries(_series).IsPublic().Build(25);
            SeriesBuilder.WithLibrary(LibraryId).WithBooks(3).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?pageNumber=3&pageSize=10&seriesid={_series.Id}");
            _assert = new PagingAssert<BookView>(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books", 3, 10,
                new KeyValuePair<string, string>("seriesId", _series.Id.ToString()));
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
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/books", 2, 10,
                new KeyValuePair<string, string>("seriesId", _series.Id.ToString()));
        }

        [Test]
        public void ShouldReturnCorrectPage()
        {
            _assert.ShouldHavePage(3)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(_SeriesBooks.Count())
                   .ShouldHaveItems(5);
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            var expectedItems = _SeriesBooks.OrderBy(a => a.Title).Skip(2 * 10).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatch(item, DatabaseConnection, LibraryId)
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
