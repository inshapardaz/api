using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.AddBookToFavorite
{
    [TestFixture]
    public class WhenAddBookToFavoriteAlreadyInFavorite : TestBase
    {
        private HttpResponseMessage _response;
        private BookDto _book;

        public WhenAddBookToFavoriteAlreadyInFavorite()
            : base(Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var books = BookBuilder.WithLibrary(LibraryId)
                                    .WithCategories(1)
                                    .HavingSeries()
                                    .AddToFavorites(UserId)
                                    .Build(2);

            _book = books.PickRandom();

            _response = await Client.PostObject<object>($"/library/{LibraryId}/favorites/books/{_book.Id}", new object());
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
        public void ShouldBeAddedToFavorites()
        {
            BookAssert.ShouldBeAddedToFavorite(_book.Id, UserId, DatabaseConnection);
        }
    }
}
