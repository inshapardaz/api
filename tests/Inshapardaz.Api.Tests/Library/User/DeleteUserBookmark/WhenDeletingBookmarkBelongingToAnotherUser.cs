using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.DeleteUserBookmark
{
    // Deleting is scoped by the caller's own AccountId, so a request for another user's
    // ClientId matches no row of the caller's own and simply no-ops (still 204, matching
    // WhenDeletingBookmarkThatDoesNotExist) - the other user's bookmark must survive untouched.
    [TestFixture]
    public class WhenDeletingBookmarkBelongingToAnotherUser() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private BookDto _book;
        private BookmarkDto _otherUsersBookmark;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var otherAccount = AccountBuilder.As(Role.Writer).Build();
            _book = BookBuilder.WithLibrary(LibraryId).Build();

            _otherUsersBookmark = new BookmarkDto
            {
                BookId = _book.Id,
                LibraryId = LibraryId,
                AccountId = otherAccount.Id,
                ClientId = RandomData.String,
                ChapterId = RandomData.String,
                Position = RandomData.Number,
                Name = RandomData.Name,
                DateAdded = DateTime.UtcNow
            };
            BookTestRepository.AddBookmark(_otherUsersBookmark);

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/my/books/{_book.Id}/bookmarks/{_otherUsersBookmark.ClientId}");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveNoContentResult() => _response.ShouldBeNoContent();

        [Test]
        public void ShouldNotHaveDeletedOtherUsersBookmark() =>
            BookTestRepository.GetBookmark(_book.Id, _otherUsersBookmark.AccountId, _otherUsersBookmark.ClientId).Should().NotBeNull();
    }
}
