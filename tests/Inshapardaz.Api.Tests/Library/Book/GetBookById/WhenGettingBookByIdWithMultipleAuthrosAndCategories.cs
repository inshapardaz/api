using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.GetBookById
{
    [TestFixture]
    public class WhenGettingBookByIdWithMultipleAuthrosAndCategories : TestBase
    {
        private HttpResponseMessage _response;
        private BookDto _expected;
        private BookAssert _assert;
        private IEnumerable<CategoryDto> _categories;
        private IEnumerable<AuthorDto> _authors;

        public WhenGettingBookByIdWithMultipleAuthrosAndCategories() : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _authors = AuthorBuilder.WithLibrary(LibraryId).Build(3);
            _categories = CategoryBuilder.WithLibrary(LibraryId).Build(3);
            var books = BookBuilder.WithLibrary(LibraryId)
                                        .HavingSeries()
                                        .WithCategories(_categories)
                                        .WithAuthors(_authors)
                                        .WithContents(3)
                                        .WithPages()
                                        .Build(4);
            _expected = books.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_expected.Id}");
            _assert = Services.GetService<BookAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
        public void ShouldHaveChaptersLink()
        {
            _assert.ShouldHaveChaptersLink();
        }

        [Test]
        public void ShouldHaveContents()
        {
            _assert.ShouldHaveContents(haveEditableLinks: true);
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
        public void ShouldHaveUpdateLink()
        {
            _assert.ShouldHaveUpdateLink();
        }

        [Test]
        public void ShouldHaveDeleteLink()
        {
            _assert.ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHavePagesLink()
        {
            _assert.ShouldHavePagesLink();
        }

        [Test]
        public void ShouldHavePagesUploadLink()
        {
            _assert.ShouldHavePagesUploadLink();
        }

        [Test]
        public void ShouldHaveImageUploadLink()
        {
            _assert.ShouldHaveImageUpdateLink();
        }

        [Test]
        public void ShouldHaveCreateChapterLink()
        {
            _assert.ShouldHaveCreateChaptersLink();
        }

        [Test]
        public void ShouldHaveAddContentLink()
        {
            _assert.ShouldHaveAddContentLink();
        }

        [Test]
        public void ShouldHaveAddFavoriteLinks()
        {
            _assert.ShouldHaveAddFavoriteLink();
        }

        [Test]
        public void ShouldReturnCorrectBookData()
        {
            _assert.ShouldBeSameAs(_expected)
                    .ShouldBeSameCategories(_categories);
        }
    }
}
