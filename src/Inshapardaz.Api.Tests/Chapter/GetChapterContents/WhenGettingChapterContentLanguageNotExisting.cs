using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Tests.Asserts;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.GetChapterContents
{
    [TestFixture]
    public class WhenGettingChapterContentLanguageNotExisting
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenGettingChapterContentLanguageNotExisting()
            : base(Domain.Adapters.Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapter = ChapterBuilder.WithLibrary(LibraryId).Public().WithContents().Build();
            var content = ChapterBuilder.Contents.Single(x => x.ChapterId == chapter.Id);
            var file = ChapterBuilder.Files.Single(x => x.Id == content.FileId);
            var contents = FileStore.GetFile(file.FilePath, CancellationToken.None).Result;

            _response = await Client.GetAsync($"/library/{LibraryId}/books/{chapter.BookId}/chapters/{chapter.Id}/contents", "babel", file.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNotFound()
        {
            _response.ShouldBeNotFound();
        }
    }
}
