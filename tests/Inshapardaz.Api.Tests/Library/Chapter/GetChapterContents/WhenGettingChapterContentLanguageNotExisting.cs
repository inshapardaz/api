using Inshapardaz.Api.Tests.Framework.Asserts;
using NUnit.Framework;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.GetChapterContents
{
    [TestFixture]
    public class WhenGettingChapterContentLanguageNotExisting() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapter = ChapterBuilder.WithLibrary(LibraryId).Public().WithContents().Build();
            var content = ChapterBuilder.Contents.Single(x => x.ChapterId == chapter.Id);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{chapter.BookId}/chapters/{chapter.Id}/contents?language=babel");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnNotFound() => _response.ShouldBeNotFound();
    }
}
