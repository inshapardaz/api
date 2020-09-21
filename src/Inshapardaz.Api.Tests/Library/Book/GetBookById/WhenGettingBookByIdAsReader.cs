using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.GetBookById
{
    [TestFixture]
    public class WhenGettingBookByIdAsReader : TestBase
    {
        private HttpResponseMessage _response;
        private BookDto _expected;
        private BookAssert _assert;
        private IEnumerable<CategoryDto> _categories;

        public WhenGettingBookByIdAsReader() : base(Domain.Adapters.Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).Build(2);
            var books = BookBuilder.WithLibrary(LibraryId)
                                        .HavingSeries()
                                        .WithCategories(_categories)
                                        .WithContents(2)
                                        .Build(4);
            _expected = books.PickRandom();

            _response = await Client.GetAsync($"/library/{LibraryId}/books/{_expected.Id}");
            _assert = BookAssert.WithResponse(_response).InLibrary(LibraryId);
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
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldHaveAuthorLink()
        {
            _assert.ShouldHaveAuthorLink();
        }

        [Test]
        public void ShouldHaveChaptersLink()
        {
            _assert.ShouldHaveChaptersLink();
        }

        [Test]
        public void ShouldHaveContents()
        {
            _assert.ShouldHaveContents(DatabaseConnection);
        }

        [Test]
        public void ShouldHaveImageLink()
        {
            _assert.ShouldHavePublicImageLink();
        }

        [Test]
        public void ShouldHaveSeriesLink()
        {
            _assert.ShouldHaveSeriesLink();
        }

        [Test]
        public void ShouldHaveAddFavoriteLinks()
        {
            _assert.ShouldHaveAddFavoriteLink();
        }

        [Test]
        public void ShouldReturnCorrectBookData()
        {
            _assert.ShouldBeSameAs(_expected, DatabaseConnection)
                    .ShouldBeSameCategories(_categories);
        }
    }
}
