using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.GetUserBookmarks
{
    [TestFixture]
    public class WhenGettingUserBookmarksWithNoBookmarks() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private BookmarkAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/my/books/{book.Id}/bookmarks");
            _assert = Services.GetService<BookmarkAssert>().ForListResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveNoBookmarks() => _assert.ShouldHaveCount(0);
    }
}
