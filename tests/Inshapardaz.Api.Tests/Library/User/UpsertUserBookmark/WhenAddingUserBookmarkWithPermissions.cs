using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpsertUserBookmark
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenAddingUserBookmarkWithPermissions(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private BookmarkAssert _assert;
        private BookDto _book;
        private string _clientId;
        private BookmarkView _payload;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).Build();
            _clientId = RandomData.String;

            _payload = new BookmarkView
            {
                ChapterId = RandomData.String,
                Position = RandomData.Number,
                Name = RandomData.Name
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/my/books/{_book.Id}/bookmarks/{_clientId}", _payload);
            _assert = Services.GetService<BookmarkAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnSavedBookmark() => _assert.ShouldMatch(new BookmarkView { Id = _clientId, ChapterId = _payload.ChapterId, Position = _payload.Position, Name = _payload.Name });

        [Test]
        public void ShouldHaveSavedBookmark() => _assert.ShouldHaveSaved(_book.Id, AccountId, _clientId, _payload)
            .ShouldHaveBeenAdded(_book.Id, AccountId, _clientId);
    }
}
