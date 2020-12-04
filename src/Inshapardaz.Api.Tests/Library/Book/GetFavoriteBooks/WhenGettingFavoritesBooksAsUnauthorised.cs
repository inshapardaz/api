using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.GetFavoriteBooks
{
    [TestFixture]
    public class WhenGettingFavoritesBooksAsUnauthorised
        : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private IEnumerable<BookDto> _favoriteBooks;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _favoriteBooks = BookBuilder.WithLibrary(LibraryId)
                                       .IsPublic()
                                       .AddToFavorites(AccountId)
                                       .Build(5);
            BookBuilder.WithLibrary(LibraryId).IsPublic().Build(3);

            _response = await Client.GetAsync($"/library/{LibraryId}/books?pageNumber=1&pageSize=10&favorite=true");
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
            _assert.ShouldHaveSelfLink($"/library/{LibraryId}/books", 1, 10, "favorite", bool.TrueString);
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
