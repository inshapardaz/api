using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Api.Tests.Framework.Asserts;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpsertUserBookmark
{
    [TestFixture]
    public class WhenAddingUserBookmarkForNonExistingBook() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var payload = new BookmarkView
            {
                ChapterId = RandomData.String,
                Position = RandomData.Number,
                Name = RandomData.Name
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/my/books/{-RandomData.Number}/bookmarks/{RandomData.String}", payload);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveNotFoundResult() => _response.ShouldBeNotFound();
    }
}
