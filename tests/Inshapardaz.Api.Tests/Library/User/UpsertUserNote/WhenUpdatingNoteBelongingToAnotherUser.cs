using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpsertUserNote
{
    // A user can never update another user's note - even by guessing their ClientId, PUTting
    // to it only ever creates/updates the caller's own note (upserts are scoped by the caller's
    // own AccountId), leaving the other user's note completely untouched.
    [TestFixture]
    public class WhenUpdatingNoteBelongingToAnotherUser() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private NoteAssert _assert;
        private BookDto _book;
        private NoteDto _otherUsersNote;
        private NoteView _payload;

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

            _payload = new NoteView
            {
                ChapterId = RandomData.String,
                StartOffset = RandomData.Number,
                EndOffset = RandomData.Number,
                Text = RandomData.Text,
                Comment = RandomData.Text
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/my/books/{_book.Id}/notes/{_otherUsersNote.ClientId}", _payload);
            _assert = Services.GetService<NoteAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveCreatedOwnNoteRatherThanUpdatingOtherUsers() =>
            _assert.ShouldHaveSaved(_book.Id, AccountId, _otherUsersNote.ClientId, _payload)
                   .ShouldHaveBeenAdded(_book.Id, AccountId, _otherUsersNote.ClientId);

        [Test]
        public void ShouldNotHaveChangedOtherUsersNote()
        {
            var otherUsersNote = BookTestRepository.GetNote(_book.Id, _otherUsersNote.AccountId, _otherUsersNote.ClientId);

            otherUsersNote.Should().NotBeNull();
            otherUsersNote.ChapterId.Should().Be(_otherUsersNote.ChapterId);
            otherUsersNote.StartOffset.Should().Be(_otherUsersNote.StartOffset);
            otherUsersNote.EndOffset.Should().Be(_otherUsersNote.EndOffset);
            otherUsersNote.Text.Should().Be(_otherUsersNote.Text);
            otherUsersNote.Comment.Should().Be(_otherUsersNote.Comment);
            otherUsersNote.DateUpdated.Should().BeNull();
        }
    }
}
