using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.DeleteUserBookmark
{
    [TestFixture]
    public class WhenDeletingBookmarkThatDoesNotExist() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/my/books/{book.Id}/bookmarks/{RandomData.String}");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnNoContent() => _response.ShouldBeNoContent();
    }
}
