using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.GetBookRating
{
    [TestFixture]
    public class WhenGettingBookRatingWithNoRatings : TestBase
    {
        private HttpResponseMessage _response;
        private RatingAssert _assert;
        private BookDto _book;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).IsPublic().Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_book.Id}/rating");
            _assert = Services.GetService<RatingAssert>().ForSummaryResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveZeroAverage() => _assert.ShouldHaveAverage(0, 0);
    }
}
