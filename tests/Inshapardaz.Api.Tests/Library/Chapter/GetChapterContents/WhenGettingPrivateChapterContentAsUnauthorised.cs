using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.GetChapterContents
{
    [TestFixture]
    public class WhenGettingPrivateChapterContentAsUnauthorised
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterDto _chapter;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _chapter = ChapterBuilder.WithLibrary(LibraryId).Private().WithContents().Build();
            var content = ChapterBuilder.Contents.Single(x => x.ChapterId == _chapter.Id);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}/contents?language={content.Language}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnUnathorised()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
