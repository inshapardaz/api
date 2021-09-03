using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.DeleteBook
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingBookWithPermission : TestBase
    {
        private HttpResponseMessage _response;

        private BookDto _expected;

        public WhenDeletingBookWithPermission(Role role) : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var books = BookBuilder.WithLibrary(LibraryId)
                                    .WithCategories(1)
                                    .HavingSeries()
                                    .AddToFavorites(AccountId)
                                    .AddToRecentReads(AccountId)
                                    .Build(1);
            _expected = books.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{_expected.Id}");
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
            BookAssert.ShouldNotBeInFavorites(_expected.Id, AccountId, DatabaseConnection);
        }

        [Test]
        public void ShouldBeDeletedFromTheRecentReadBooks()
        {
            BookAssert.ShouldHaveDeletedBookFromRecentReads(_expected.Id, DatabaseConnection);
        }
    }
}
