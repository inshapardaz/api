using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.RemoveBookFromFavorite
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    [TestFixture(Permission.Reader)]
    public class WhenRemoveBookFromFavoriteWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private BookDto _book;

        public WhenRemoveBookFromFavoriteWithPermissions(Permission permission) : base(permission)
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

            _response = await Client.DeleteAsync($"/library/{LibraryId}/favorites/books/{_book.Id}");
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
            BookAssert.ShouldNotBeInFavorites(_book.Id, UserId, DatabaseConnection);
        }
    }
}
