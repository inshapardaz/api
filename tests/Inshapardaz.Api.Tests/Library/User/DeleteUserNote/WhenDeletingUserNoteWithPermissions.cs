using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.DeleteUserNote
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenDeletingUserNoteWithPermissions(Role role) : TestBase(role)
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

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/my/books/{_book.Id}/notes/{_note.ClientId}");
            _assert = Services.GetService<NoteAssert>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveNoContentResult() => _response.ShouldBeNoContent();

        [Test]
        public void ShouldHaveDeletedNote() => _assert.ShouldNotExist(_book.Id, AccountId, _note.ClientId);
    }
}
