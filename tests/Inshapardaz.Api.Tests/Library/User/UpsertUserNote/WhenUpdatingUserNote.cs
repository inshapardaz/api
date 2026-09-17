using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpsertUserNote
{
    [TestFixture]
    public class WhenUpdatingUserNote() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private NoteAssert _assert;
        private BookDto _book;
        private NoteDto _existingNote;
        private NoteView _payload;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).Build();

            _existingNote = new NoteDto
            {
                BookId = _book.Id,
                LibraryId = LibraryId,
                AccountId = AccountId,
                ClientId = RandomData.String,
                ChapterId = RandomData.String,
                StartOffset = RandomData.Number,
                EndOffset = RandomData.Number,
                Text = RandomData.Text,
                Comment = RandomData.Text,
                DateAdded = DateTime.UtcNow
            };
            BookTestRepository.AddNote(_existingNote);

            _payload = new NoteView
            {
                ChapterId = RandomData.String,
                StartOffset = RandomData.Number,
                EndOffset = RandomData.Number,
                Text = RandomData.Text,
                Comment = RandomData.Text
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/my/books/{_book.Id}/notes/{_existingNote.ClientId}", _payload);
            _assert = Services.GetService<NoteAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnUpdatedNote() => _assert.ShouldMatch(new NoteView { Id = _existingNote.ClientId, ChapterId = _payload.ChapterId, StartOffset = _payload.StartOffset, EndOffset = _payload.EndOffset, Text = _payload.Text, Comment = _payload.Comment });

        [Test]
        public void ShouldHaveUpdatedNote() => _assert.ShouldHaveSaved(_book.Id, AccountId, _existingNote.ClientId, _payload)
            .ShouldHaveBeenUpdated(_book.Id, AccountId, _existingNote.ClientId);
    }
}
