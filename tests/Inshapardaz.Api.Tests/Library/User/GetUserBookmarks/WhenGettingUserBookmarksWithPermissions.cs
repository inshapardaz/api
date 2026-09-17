using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.GetUserBookmarks
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenGettingUserBookmarksWithPermissions(Role role) : TestBase(role)
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

            _response = await Client.GetAsync($"/libraries/{LibraryId}/my/books/{_book.Id}/bookmarks");
            _assert = Services.GetService<BookmarkAssert>().ForListResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveExpectedBookmark() => _assert.ShouldHaveCount(1)
            .ShouldContain(new BookmarkView
            {
                Id = _bookmark.ClientId,
                ChapterId = _bookmark.ChapterId,
                Position = _bookmark.Position,
                Name = _bookmark.Name
            });
    }
}
