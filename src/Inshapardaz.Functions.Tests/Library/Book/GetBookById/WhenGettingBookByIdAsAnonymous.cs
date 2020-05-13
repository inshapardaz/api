using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.GetBookById
{
    [TestFixture]
    public class WhenGettingBookByIdAsAnonymous
        : LibraryTest<Functions.Library.Books.GetBookById>
    {
        private OkObjectResult _response;
        private BookDto _expected;
        private BookAssert _assert;
        private IEnumerable<CategoryDto> _categories;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var dataBuilder = Container.GetService<BooksDataBuilder>();
            _categories = Container.GetService<CategoriesDataBuilder>().WithLibrary(LibraryId).Build(2);
            var books = dataBuilder.WithLibrary(LibraryId)
                                        .HavingSeries()
                                        .WithCategories(_categories)
                                        .Build(4);
            _expected = books.PickRandom();

            _response = (OkObjectResult)await handler.Run(request, LibraryId, _expected.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
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
        public void ShouldHaveFilesLink()
        {
            _assert.ShouldHaveFilesLink();
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
        public void ShouldReturnCorrectBookData()
        {
            _assert.ShouldBeSameAs(_expected, DatabaseConnection)
                    .ShouldBeSameCategories(_categories);
        }
    }
}
