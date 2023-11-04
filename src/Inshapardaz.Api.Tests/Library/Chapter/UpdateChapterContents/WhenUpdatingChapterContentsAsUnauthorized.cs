using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.UpdateChapterContents
{
    [TestFixture]
    public class WhenUpdatingChapterContentsAsUnauthorised
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterDto _chapter;
        private ChapterContentDto _content;

        private string _newContents;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _chapter = ChapterBuilder.WithLibrary(LibraryId).Public().WithContents().Build();
            _content = ChapterBuilder.Contents.Single(x => x.ChapterId == _chapter.Id);

            _newContents = RandomData.String;

            _response = await Client.PutString($"/libraries/{LibraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}/contents?language={_content.Language}", _newContents);
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
