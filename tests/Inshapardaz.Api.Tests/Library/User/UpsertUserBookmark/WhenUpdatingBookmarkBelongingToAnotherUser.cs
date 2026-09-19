using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpsertUserBookmark
{
    // A user can never update another user's bookmark - even by guessing their ClientId,
    // PUTting to it only ever creates/updates the caller's own bookmark (upserts are scoped by
    // the caller's own AccountId), leaving the other user's bookmark completely untouched.
    [TestFixture]
    public class WhenUpdatingBookmarkBelongingToAnotherUser() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private BookmarkAssert _assert;
        private BookDto _book;
        private BookmarkDto _otherUsersBookmark;
        private BookmarkView _payload;

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

            _payload = new BookmarkView
            {
                ChapterId = RandomData.String,
                Position = RandomData.Number,
                Name = RandomData.Name
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/my/books/{_book.Id}/bookmarks/{_otherUsersBookmark.ClientId}", _payload);
            _assert = Services.GetService<BookmarkAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveCreatedOwnBookmarkRatherThanUpdatingOtherUsers() =>
            _assert.ShouldHaveSaved(_book.Id, AccountId, _otherUsersBookmark.ClientId, _payload)
                   .ShouldHaveBeenAdded(_book.Id, AccountId, _otherUsersBookmark.ClientId);

        [Test]
        public void ShouldNotHaveChangedOtherUsersBookmark()
        {
            var otherUsersBookmark = BookTestRepository.GetBookmark(_book.Id, _otherUsersBookmark.AccountId, _otherUsersBookmark.ClientId);

            otherUsersBookmark.Should().NotBeNull();
            otherUsersBookmark.ChapterId.Should().Be(_otherUsersBookmark.ChapterId);
            otherUsersBookmark.Position.Should().Be(_otherUsersBookmark.Position);
            otherUsersBookmark.Name.Should().Be(_otherUsersBookmark.Name);
            otherUsersBookmark.DateUpdated.Should().BeNull();
        }
    }
}
