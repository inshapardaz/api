using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenGettingBooksBySeriesWithWritePermission
        : LibraryTest<Functions.Library.Books.GetBooks>
    {
        private OkObjectResult _response;
        private PagingAssert<BookView> _assert;
        private SeriesDataBuilder _builder;
        private SeriesDto _series;
        private IEnumerable<BookDto> _seriesBooks;
        private ClaimsPrincipal _claim;

        public WhenGettingBooksBySeriesWithWritePermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<SeriesDataBuilder>();
            var _bookBuilder = Container.GetService<BooksDataBuilder>();
            _series = _builder.WithLibrary(LibraryId).Build();
            _seriesBooks = _bookBuilder.WithLibrary(LibraryId).WithSeries(_series).IsPublic().Build(5);
            _builder.WithLibrary(LibraryId).WithBooks(3).Build();

            var request = new RequestBuilder()
                          .WithQueryParameter("pageNumber", 1)
                          .WithQueryParameter("pageSize", 10)
                          .WithQueryParameter("seriesid", _series.Id)
                          .Build();

            _response = (OkObjectResult)await handler.Run(request, LibraryId, _claim, CancellationToken.None);

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
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/books", 1, 10, "seriesid", _series.Id.ToString());
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
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldHaveCreateLink($"/api/library/{LibraryId}/books");
        }

        [Test]
        public void ShouldReturnCorrectPage()
        {
            _assert.ShouldHavePage(1)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(_seriesBooks.Count())
                   .ShouldHaveItems(5);
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            var expectedItems = _seriesBooks.OrderBy(a => a.Title).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatch(item, DatabaseConnection)
                            .InLibrary(LibraryId)
                            .ShouldHaveCorrectLinks()
                            .ShouldHaveSeriesLink()
                            .ShouldHaveEditLinks()
                            .ShouldHaveImageUpdateLink()
                            .ShouldHaveCreateChaptersLink()
                            .ShouldHaveAddContentLink()
                            .ShouldHaveChaptersLink()
                            .ShouldHavePublicImageLink();
            }
        }
    }
}
