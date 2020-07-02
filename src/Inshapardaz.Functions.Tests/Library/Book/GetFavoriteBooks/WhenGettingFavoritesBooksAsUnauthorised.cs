using System.Collections.Generic;
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

namespace Inshapardaz.Functions.Tests.Library.Book.GetFavoriteBooks
{
    [TestFixture]
    public class WhenGettingFavoritesBooksAsUnauthorised
        : LibraryTest<Functions.Library.Books.GetBooks>
    {
        private OkObjectResult _response;
        private PagingAssert<BookView> _assert;
        private IEnumerable<BookDto> _favoriteBooks;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var claim = AuthenticationBuilder.ReaderClaim;
            var _bookBuilder = Container.GetService<BooksDataBuilder>();
            _favoriteBooks = _bookBuilder.WithLibrary(LibraryId)
                                       .IsPublic()
                                       .AddToFavorites(claim.GetUserId())
                                       .Build(5);
            _bookBuilder.WithLibrary(LibraryId).IsPublic().Build(3);

            var request = new RequestBuilder()
                          .WithQueryParameter("pageNumber", 1)
                          .WithQueryParameter("pageSize", 10)
                          .WithQueryParameter("favorite", bool.TrueString)
                          .Build();

            _response = (OkObjectResult)await handler.Run(request, LibraryId, AuthenticationBuilder.Unauthorized, CancellationToken.None);

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
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/books", 1, 10, "favorite", bool.TrueString);
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
            _assert.ShouldHavePage(1)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(0)
                   .ShouldHaveItems(0);
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            _assert.ShouldHaveNoData();
            //var expectedItems = _favoriteBooks.OrderBy(a => a.Title).Take(10);
            //foreach (var item in expectedItems)
            //{
            //    var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
            //    actual.ShouldMatch(item, DatabaseConnection)
            //                .InLibrary(LibraryId)
            //                .ShouldHaveCorrectLinks()
            //                .ShouldNotHaveSeriesLink()
            //                .ShouldNotHaveEditLinks()
            //                .ShouldNotHaveImageUpdateLink()
            //                .ShouldNotHaveCreateChaptersLink()
            //                .ShouldNotHaveAddContentLink()
            //                .ShouldNotHaveAddFavoriteLink()
            //                .ShouldNotHaveRemoveFavoriteLink()
            //                .ShouldHaveChaptersLink()
            //                .ShouldHavePublicImageLink();
        }
    }
}
