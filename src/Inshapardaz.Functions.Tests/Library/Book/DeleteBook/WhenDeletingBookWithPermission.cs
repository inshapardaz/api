using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.DeleteBook
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenDeletingBookWithPermission : LibraryTest<Functions.Library.Books.DeleteBook>
    {
        private readonly ClaimsPrincipal _claim;
        private OkResult _response;

        private BookDto _expected;
        private BooksDataBuilder _dataBuilder;

        public WhenDeletingBookWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();
            var books = _dataBuilder.WithLibrary(LibraryId)
                                    .WithCategories(1)
                                    .HavingSeries()
                                    .AddToFavorites(Guid.NewGuid())
                                    .AddToRecentReads(Guid.NewGuid())
                                    .Build(1);
            _expected = books.First();

            var request = TestHelpers.CreateGetRequest();
            _response = (OkResult)await handler.Run(request, LibraryId, _expected.Id, _claim, CancellationToken.None);
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
        public void ShouldHaveDeletedBook()
        {
            BookAssert.ShouldHaveDeletedBook(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldHaveDeletedTheBookImage()
        {
            BookAssert.ShouldHaveDeletedBookImage(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldNotHaveDeletedTheAuthor()
        {
            AuthorAssert.ShouldNotHaveDeletedAuthor(_expected.AuthorId, DatabaseConnection);
        }

        [Test]
        public void ShouldNotHaveDeletedTheSeries()
        {
            SeriesAssert.ShouldNotHaveDeletedSeries(_expected.SeriesId.Value, DatabaseConnection);
        }

        [Test]
        public void ShouldNotHaveDeletedTheCategory()
        {
            var cats = DatabaseConnection.GetCategoriesByBook(_expected.Id);
            cats.ForEach(cat => CategoryAssert.ShouldNotHaveDeletedCategory(LibraryId, cat.Id, DatabaseConnection));
        }

        [Test]
        public void ShouldBeDeletedFromTheFavoritesOfAllUsers()
        {
            BookAssert.ShouldHaveDeletedBookFromFavorites(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldBeDeletedFromTheRecentReadBooks()
        {
            BookAssert.ShouldHaveDeletedBookFromRecentReads(_expected.Id, DatabaseConnection);
        }
    }
}
