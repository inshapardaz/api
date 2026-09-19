using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Api.Tests.Framework.Asserts;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpsertUserBookRating
{
    [TestFixture]
    public class WhenAddingUserBookRatingAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();
            var payload = new RatingView
            {
                Value = 3
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/my/books/{book.Id}/rating", payload);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveUnauthorizedResult() => _response.ShouldBeUnauthorized();
    }
}
