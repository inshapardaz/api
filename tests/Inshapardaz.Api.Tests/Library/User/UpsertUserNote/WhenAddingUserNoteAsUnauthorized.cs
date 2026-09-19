using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Api.Tests.Framework.Asserts;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpsertUserNote
{
    [TestFixture]
    public class WhenAddingUserNoteAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();
            var payload = new NoteView
            {
                ChapterId = RandomData.String,
                StartOffset = RandomData.Number,
                EndOffset = RandomData.Number,
                Text = RandomData.Text,
                Comment = RandomData.Text
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/my/books/{book.Id}/notes/{RandomData.String}", payload);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveUnauthorizedResult() => _response.ShouldBeUnauthorized();
    }
}
