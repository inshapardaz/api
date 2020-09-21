using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.DeleteChapterContents
{
    [TestFixture]
    public class WhenDeletingChapterContentsAsReader
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenDeletingChapterContentsAsReader()
            : base(Domain.Adapters.Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var _newContents = Random.Words(12);
            var chapter = ChapterBuilder.WithLibrary(LibraryId).WithContents().Build();
            var content = ChapterBuilder.Contents.Single(x => x.ChapterId == chapter.Id);
            var file = ChapterBuilder.Files.Single(x => x.Id == content.FileId);

            _response = await Client.DeleteAsync($"/library/{LibraryId}/books/{chapter.BookId}/chapters/{chapter.Id}/contents", content.Language, file.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}
