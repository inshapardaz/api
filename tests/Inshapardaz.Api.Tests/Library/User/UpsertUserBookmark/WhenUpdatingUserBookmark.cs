using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpsertUserBookmark
{
    [TestFixture]
    public class WhenUpdatingUserBookmark() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private BookmarkAssert _assert;
        private BookDto _book;
        private BookmarkDto _existingBookmark;
        private BookmarkView _payload;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).Build();

            _existingBookmark = new BookmarkDto
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
            BookTestRepository.AddBookmark(_existingBookmark);

            _payload = new BookmarkView
            {
                ChapterId = RandomData.String,
                Position = RandomData.Number,
                Name = RandomData.Name
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/my/books/{_book.Id}/bookmarks/{_existingBookmark.ClientId}", _payload);
            _assert = Services.GetService<BookmarkAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnUpdatedBookmark() => _assert.ShouldMatch(new BookmarkView { Id = _existingBookmark.ClientId, ChapterId = _payload.ChapterId, Position = _payload.Position, Name = _payload.Name });

        [Test]
        public void ShouldHaveUpdatedBookmark() => _assert.ShouldHaveSaved(_book.Id, AccountId, _existingBookmark.ClientId, _payload)
            .ShouldHaveBeenUpdated(_book.Id, AccountId, _existingBookmark.ClientId);
    }
}
