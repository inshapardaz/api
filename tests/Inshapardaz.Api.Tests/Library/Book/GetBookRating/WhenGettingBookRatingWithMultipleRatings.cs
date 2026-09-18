using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.GetBookRating
{
    [TestFixture]
    public class WhenGettingBookRatingWithMultipleRatings() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private RatingAssert _assert;
        private BookDto _book;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).Build();

            var firstAccount = AccountBuilder.As(Role.Reader).Build();
            var secondAccount = AccountBuilder.As(Role.Reader).Build();

            BookTestRepository.AddRating(new RatingDto { BookId = _book.Id, LibraryId = LibraryId, AccountId = AccountId, Value = 5, DateAdded = DateTime.UtcNow });
            BookTestRepository.AddRating(new RatingDto { BookId = _book.Id, LibraryId = LibraryId, AccountId = firstAccount.Id, Value = 3, DateAdded = DateTime.UtcNow });
            BookTestRepository.AddRating(new RatingDto { BookId = _book.Id, LibraryId = LibraryId, AccountId = secondAccount.Id, Value = 4, DateAdded = DateTime.UtcNow });

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_book.Id}/rating");
            _assert = Services.GetService<RatingAssert>().ForSummaryResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveAverageOfAllRatings() => _assert.ShouldHaveAverage(4.0, 3);
    }
}
