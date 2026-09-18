using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpsertUserBookRating
{
    [TestFixture]
    public class WhenAddingUserBookRatingForNonExistingBook() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var payload = new RatingView
            {
                Value = 3
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/my/books/{-RandomData.Number}/rating", payload);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveNotFoundResult() => _response.ShouldBeNotFound();
    }
}
