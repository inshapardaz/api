using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.DeleteUserBookmark
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenDeletingUserBookmarkWithPermissions(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private BookmarkAssert _assert;
        private BookDto _book;
        private BookmarkDto _bookmark;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).Build();

            _bookmark = new BookmarkDto
            {
                BookId = _book.Id,
                LibraryId = LibraryId,
                AccountId = AccountId,
                ClientId = RandomData.String,
                ChapterId = RandomData.String,
                Position = RandomData.Number,
                Name = RandomData.Name,
                DateAdded = DateTime.UtcNow
            };
            BookTestRepository.AddBookmark(_bookmark);

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/my/books/{_book.Id}/bookmarks/{_bookmark.ClientId}");
            _assert = Services.GetService<BookmarkAssert>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveNoContentResult() => _response.ShouldBeNoContent();

        [Test]
        public void ShouldHaveDeletedBookmark() => _assert.ShouldNotExist(_book.Id, AccountId, _bookmark.ClientId);
    }
}
