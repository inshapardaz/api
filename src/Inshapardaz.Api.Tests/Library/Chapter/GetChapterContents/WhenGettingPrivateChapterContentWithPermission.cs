using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.GetChapterContents
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenGettingPrivateChapterContentWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterContentAssert _assert;
        private ChapterDto _chapter;
        private ChapterContentDto _content;

        public WhenGettingPrivateChapterContentWithPermission(Permission permission)
            : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _chapter = ChapterBuilder.WithLibrary(LibraryId).Private().WithContents().Build();
            _content = ChapterBuilder.Contents.Single(x => x.ChapterId == _chapter.Id);
            var file = ChapterBuilder.Files.Single(x => x.Id == _content.FileId);
            var contents = FileStore.GetFile(file.FilePath, CancellationToken.None).Result;

            _response = await Client.GetAsync($"/library/{LibraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}/contents", _content.Language, file.MimeType);
            _assert = new ChapterContentAssert(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldHaveBookLink()
        {
            _assert.ShouldHaveBookLink();
        }

        [Test]
        public void ShouldHaveChapterLink()
        {
            _assert.ShouldHaveChapterLink();
        }

        [Test]
        public void ShouldHaveDownloadLink()
        {
            _assert.ShouldHavePrivateDownloadLink();
        }

        [Test]
        public void ShouldHaveEditLinks()
        {
            _assert.ShouldHaveUpdateLink();
            _assert.ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldReturnCorrectChapterData()
        {
            _assert.ShouldMatch(_content, _chapter.BookId, DatabaseConnection);
        }
    }
}
