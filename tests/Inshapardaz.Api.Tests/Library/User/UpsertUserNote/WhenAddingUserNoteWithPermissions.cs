using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpsertUserNote
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenAddingUserNoteWithPermissions(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private NoteAssert _assert;
        private BookDto _book;
        private string _clientId;
        private NoteView _payload;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).Build();
            _clientId = RandomData.String;

            _payload = new NoteView
            {
                ChapterId = RandomData.String,
                StartOffset = RandomData.Number,
                EndOffset = RandomData.Number,
                Text = RandomData.Text,
                Comment = RandomData.Text
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/my/books/{_book.Id}/notes/{_clientId}", _payload);
            _assert = Services.GetService<NoteAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnSavedNote() => _assert.ShouldMatch(new NoteView { Id = _clientId, ChapterId = _payload.ChapterId, StartOffset = _payload.StartOffset, EndOffset = _payload.EndOffset, Text = _payload.Text, Comment = _payload.Comment });

        [Test]
        public void ShouldHaveSavedNote() => _assert.ShouldHaveSaved(_book.Id, AccountId, _clientId, _payload)
            .ShouldHaveBeenAdded(_book.Id, AccountId, _clientId);
    }
}
