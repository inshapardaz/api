using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.GetBooks
{
    [TestFixture]
    public class WhenGettingBooksBySeriesNameAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private SeriesDto _series;
        private IEnumerable<BookDto> _seriesBooks;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _series = SeriesBuilder.WithLibrary(LibraryId).Build();
            _seriesBooks = BookBuilder.WithLibrary(LibraryId).WithSeries(_series).IsPublic().Build(3);
            BookBuilder.WithLibrary(LibraryId).IsPublic().Build(10);

            var namePart = _series.Name.Substring(0, _series.Name.Length / 2);
            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?seriesName={Uri.EscapeDataString(namePart)}");

            _assert = Services.GetService<PagingAssert<BookView>>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnOnlyBooksInMatchingSeries()
        {
            _assert.ShouldHaveTotalCount(_seriesBooks.Count())
                   .ShouldHaveItems(_seriesBooks.Count());
        }
    }
}
