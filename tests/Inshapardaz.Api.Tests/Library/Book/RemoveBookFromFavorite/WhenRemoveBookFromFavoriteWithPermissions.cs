using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.RemoveBookFromFavorite
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenRemoveBookFromFavoriteWithPermissions(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private BookAssert _assert;
        private BookDto _book;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var books = BookBuilder.WithLibrary(LibraryId)
                                    .WithCategories(1)
                                    .HavingSeries()
                                    .AddToFavorites(AccountId)
                                    .Build(2);

            _book = books.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/favorites/books/{_book.Id}");
            _assert = Services.GetService<BookAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOKResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldBeRemovedFromFavorites() => _assert.ShouldNotBeInFavorites(_book.Id, AccountId);
    }
}
