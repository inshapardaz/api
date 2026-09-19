using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.DeleteUserNote
{
    // Deleting is scoped by the caller's own AccountId, so a request for another user's
    // ClientId matches no row of the caller's own and simply no-ops (still 204, matching
    // WhenDeletingNoteThatDoesNotExist) - the other user's note must survive untouched.
    [TestFixture]
    public class WhenDeletingNoteBelongingToAnotherUser() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private BookDto _book;
        private NoteDto _otherUsersNote;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var otherAccount = AccountBuilder.As(Role.Writer).Build();
            _book = BookBuilder.WithLibrary(LibraryId).Build();

            _otherUsersNote = new NoteDto
            {
                BookId = _book.Id,
                LibraryId = LibraryId,
                AccountId = otherAccount.Id,
                ClientId = RandomData.String,
                ChapterId = RandomData.String,
                StartOffset = RandomData.Number,
                EndOffset = RandomData.Number,
                Text = RandomData.Text,
                Comment = RandomData.Text,
                DateAdded = DateTime.UtcNow
            };
            BookTestRepository.AddNote(_otherUsersNote);

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/my/books/{_book.Id}/notes/{_otherUsersNote.ClientId}");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveNoContentResult() => _response.ShouldBeNoContent();

        [Test]
        public void ShouldNotHaveDeletedOtherUsersNote() =>
            BookTestRepository.GetNote(_book.Id, _otherUsersNote.AccountId, _otherUsersNote.ClientId).Should().NotBeNull();
    }
}
