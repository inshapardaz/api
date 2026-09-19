using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpsertUserBookRating
{
    [TestFixture]
    public class WhenUpdatingUserBookRating() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private RatingAssert _assert;
        private BookDto _book;
        private RatingDto _existingRating;
        private RatingView _payload;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).Build();

            _existingRating = new RatingDto
            {
                BookId = _book.Id,
                LibraryId = LibraryId,
                AccountId = AccountId,
                Value = 2,
                DateAdded = DateTime.UtcNow
            };
            BookTestRepository.AddRating(_existingRating);

            _payload = new RatingView
            {
                Value = 5
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/my/books/{_book.Id}/rating", _payload);
            _assert = Services.GetService<RatingAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnUpdatedRating() => _assert.ShouldMatch(_payload.Value);

        [Test]
        public void ShouldHaveUpdatedRating() => _assert.ShouldHaveSaved(_book.Id, AccountId, _payload.Value)
            .ShouldHaveBeenUpdated(_book.Id, AccountId);
    }
}
