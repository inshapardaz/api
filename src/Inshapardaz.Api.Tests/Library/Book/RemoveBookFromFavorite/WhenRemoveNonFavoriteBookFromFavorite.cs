using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.RemoveBookFromFavorite
{
    [TestFixture]
    public class WhenRemoveNonFavoriteBookFromFavorite : TestBase
    {
        private HttpResponseMessage _response;
        private BookDto _book;

        public WhenRemoveNonFavoriteBookFromFavorite()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var books = BookBuilder.WithLibrary(LibraryId)
                                    .WithCategories(1)
                                    .HavingSeries()
                                    .Build(2);

            _book = books.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/favorites/books/{_book.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOKResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldBeRemovedFromFavorites()
        {
            BookAssert.ShouldNotBeInFavorites(_book.Id, AccountId, DatabaseConnection);
        }
    }
}