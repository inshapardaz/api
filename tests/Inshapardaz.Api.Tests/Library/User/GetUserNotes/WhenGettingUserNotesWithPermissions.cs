using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.GetUserNotes
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenGettingUserNotesWithPermissions(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private NoteAssert _assert;
        private BookDto _book;
        private NoteDto _note;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).Build();

            _note = new NoteDto
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
            BookTestRepository.AddNote(_note);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/my/books/{_book.Id}/notes");
            _assert = Services.GetService<NoteAssert>().ForListResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveExpectedNote() => _assert.ShouldHaveCount(1)
            .ShouldContain(new NoteView
            {
                Id = _note.ClientId,
                ChapterId = _note.ChapterId,
                StartOffset = _note.StartOffset,
                EndOffset = _note.EndOffset,
                Text = _note.Text,
                Comment = _note.Comment
            });
    }
}
