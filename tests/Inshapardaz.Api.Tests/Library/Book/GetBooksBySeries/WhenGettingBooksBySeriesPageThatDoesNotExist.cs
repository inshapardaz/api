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

namespace Inshapardaz.Api.Tests.Library.Book.GetBooksBySeries
{
    [TestFixture]
    public class WhenGettingBooksBySeriesPageThatDoesNotExist
        : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private SeriesDto _series;
        private IEnumerable<BookDto> _SeriesBooks;

        public WhenGettingBooksBySeriesPageThatDoesNotExist()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _series = SeriesBuilder.WithLibrary(LibraryId).Build();
            _SeriesBooks = BookBuilder.WithLibrary(LibraryId).WithSeries(_series).IsPublic().Build(25);
            SeriesBuilder.WithLibrary(LibraryId).WithBooks(3).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?pageNumber=4&pageSize=10&seriesid={_series.Id}");
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books", 4, 10,
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
