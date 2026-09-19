using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Tests.Framework.Asserts;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.DeleteUserBookmark
{
    [TestFixture]
    public class WhenDeletingUserBookmarkAsUnauthorized : TestBase
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
        public void ShouldHaveUnauthorizedResult() => _response.ShouldBeUnauthorized();
    }
}
