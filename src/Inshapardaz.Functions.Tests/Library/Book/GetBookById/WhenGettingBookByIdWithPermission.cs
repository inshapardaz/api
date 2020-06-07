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

namespace Inshapardaz.Functions.Tests.Library.Book.GetBookById
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenGettingBookByIdWithPermission : LibraryTest<Functions.Library.Books.GetBookById>
    {
        private readonly ClaimsPrincipal _claim;
        private OkObjectResult _response;
        private BookDto _expected;
        private BookAssert _assert;
        private IEnumerable<CategoryDto> _categories;

        public WhenGettingBookByIdWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var dataBuilder = Container.GetService<BooksDataBuilder>();
            _categories = Container.GetService<CategoriesDataBuilder>().WithLibrary(LibraryId).Build(2);
            var books = dataBuilder.WithLibrary(LibraryId)
                                        .HavingSeries()
                                        .WithCategories(_categories)
                                        .WithContents(3)
                                        .Build(4);
            _expected = books.PickRandom();

            _response = (OkObjectResult)await handler.Run(request, LibraryId, _expected.Id, _claim, CancellationToken.None);
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
            _assert.ShouldHaveContents(DatabaseConnection, haveEditableLinks: true);
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
        public void ShouldReturnCorrectBookData()
        {
            _assert.ShouldBeSameAs(_expected, DatabaseConnection)
                    .ShouldBeSameCategories(_categories);
        }
    }
}
