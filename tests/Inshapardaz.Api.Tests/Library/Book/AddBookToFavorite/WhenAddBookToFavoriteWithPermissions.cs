using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.AddBookToFavorite
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenAddBookToFavoriteWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private BookAssert _bookAssert;
        private BookDto _book;

        public WhenAddBookToFavoriteWithPermissions(Role role) : base(role)
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

            _response = await Client.PostObject<object>($"/libraries/{LibraryId}/favorites/books/{_book.Id}", new object());
            _bookAssert = Services.GetService<BookAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _bookAssert.ShouldBeAddedToFavorite(_book.Id, AccountId);
        }
    }
}
