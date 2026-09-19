using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.GetUserBookRating
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenGettingUserBookRatingWithPermissions(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private RatingAssert _assert;
        private BookDto _book;
        private RatingDto _rating;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).Build();

            _rating = new RatingDto
            {
                BookId = _book.Id,
                LibraryId = LibraryId,
                AccountId = AccountId,
                Value = 3,
                DateAdded = DateTime.UtcNow
            };
            BookTestRepository.AddRating(_rating);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/my/books/{_book.Id}/rating");
            _assert = Services.GetService<RatingAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveExpectedRating() => _assert.ShouldMatch(_rating.Value);
    }
}
