using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.GetUserBookRating
{
    // GET only ever returns the caller's own rating - another account's rating on the same book
    // must not leak through, even though both are rows for the same BookId.
    [TestFixture]
    public class WhenGettingRatingBelongingToAnotherUser() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private BookDto _book;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var otherAccount = AccountBuilder.As(Role.Writer).Build();
            _book = BookBuilder.WithLibrary(LibraryId).Build();

            BookTestRepository.AddRating(new RatingDto
            {
                BookId = _book.Id,
                LibraryId = LibraryId,
                AccountId = otherAccount.Id,
                Value = 5,
                DateAdded = DateTime.UtcNow
            });

            _response = await Client.GetAsync($"/libraries/{LibraryId}/my/books/{_book.Id}/rating");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldNotSeeOtherUsersRating() => _response.ShouldBeNotFound();
    }
}
