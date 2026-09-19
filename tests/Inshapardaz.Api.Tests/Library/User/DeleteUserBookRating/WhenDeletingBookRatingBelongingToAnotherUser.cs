using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Api.Tests.Framework.Asserts;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.DeleteUserBookRating
{
    // Deleting is scoped by the caller's own AccountId, so it can never remove another
    // account's rating on the same book, even though both are rows for the same BookId.
    [TestFixture]
    public class WhenDeletingBookRatingBelongingToAnotherUser() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private BookDto _book;
        private RatingDto _otherUsersRating;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var otherAccount = AccountBuilder.As(Role.Writer).Build();
            _book = BookBuilder.WithLibrary(LibraryId).Build();

            _otherUsersRating = new RatingDto
            {
                BookId = _book.Id,
                LibraryId = LibraryId,
                AccountId = otherAccount.Id,
                Value = 4,
                DateAdded = DateTime.UtcNow
            };
            BookTestRepository.AddRating(_otherUsersRating);

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/my/books/{_book.Id}/rating");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveNoContentResult() => _response.ShouldBeNoContent();

        [Test]
        public void ShouldNotHaveDeletedOtherUsersRating() =>
            BookTestRepository.GetRating(_book.Id, _otherUsersRating.AccountId).Should().NotBeNull();
    }
}
