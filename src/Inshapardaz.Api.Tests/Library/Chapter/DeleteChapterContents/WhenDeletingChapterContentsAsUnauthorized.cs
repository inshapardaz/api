using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.DeleteChapterContents
{
    [TestFixture]
    public class WhenDeletingChapterContentsAsUnauthorized
        : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapter = ChapterBuilder.WithLibrary(LibraryId).WithContents().Build();
            var content = ChapterBuilder.Contents.Single(x => x.ChapterId == chapter.Id);

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{chapter.BookId}/chapters/{chapter.ChapterNumber}/contents?language={content.Language}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
