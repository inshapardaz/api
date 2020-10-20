using System;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.DeleteBook
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenDeletingBookWithPermission : TestBase
    {
        private HttpResponseMessage _response;

        private BookDto _expected;

        public WhenDeletingBookWithPermission(Permission permission) : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var books = BookBuilder.WithLibrary(LibraryId)
                                    .WithCategories(1)
                                    .HavingSeries()
                                    .AddToFavorites(Guid.NewGuid())
                                    .AddToRecentReads(Guid.NewGuid())
                                    .Build(1);
            _expected = books.PickRandom();

            _response = await Client.DeleteAsync($"/library/{LibraryId}/books/{_expected.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
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
            BookAssert.ShouldNotBeInFavorites(_expected.Id, UserId, DatabaseConnection);
        }

        [Test]
        public void ShouldBeDeletedFromTheRecentReadBooks()
        {
            BookAssert.ShouldHaveDeletedBookFromRecentReads(_expected.Id, DatabaseConnection);
        }
    }
}
