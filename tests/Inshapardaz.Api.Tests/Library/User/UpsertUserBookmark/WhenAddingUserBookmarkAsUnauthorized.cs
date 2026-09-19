using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Api.Tests.Framework.Asserts;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpsertUserBookmark
{
    [TestFixture]
    public class WhenAddingUserBookmarkAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();
            var payload = new BookmarkView
            {
                ChapterId = RandomData.String,
                Position = RandomData.Number,
                Name = RandomData.Name
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/my/books/{book.Id}/bookmarks/{RandomData.String}", payload);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveUnauthorizedResult() => _response.ShouldBeUnauthorized();
    }
}
