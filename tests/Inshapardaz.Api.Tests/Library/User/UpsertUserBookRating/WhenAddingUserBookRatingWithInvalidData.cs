using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpsertUserBookRating
{
    [TestFixture]
    public class WhenAddingUserBookRatingWithInvalidData() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();
            var payload = new RatingView
            {
                Value = 6 // out of the valid 1-5 range
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/my/books/{book.Id}/rating", payload);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveBadRequestResult() => _response.ShouldBeBadRequest();
    }
}
