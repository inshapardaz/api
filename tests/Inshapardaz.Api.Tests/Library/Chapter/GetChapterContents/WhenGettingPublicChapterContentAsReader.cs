using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.GetChapterContents
{
    [TestFixture]
    public class WhenGettingPublicChapterContentAsReader
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterContentAssert _assert;
        private ChapterDto _chapter;
        private ChapterContentDto _content;

        public WhenGettingPublicChapterContentAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _chapter = ChapterBuilder.WithLibrary(LibraryId).Public().WithContents().Build();
            _content = ChapterBuilder.Contents.Single(x => x.ChapterId == _chapter.Id);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/contents?language={_content.Language}");
            _assert = Services.GetService<ChapterContentAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
        public void ShouldHaveContentLink()
        {
            _assert.ShouldHaveContentLink();
        }

        [Test]
        public void ShouldHaveTextReturened()
        {
            var file = FileTestRepository.GetFileById(_content.FileId);
            var contents = FileStore.GetTextFile(file.FilePath, CancellationToken.None).Result;
            _assert.ShouldHaveText(contents);
        }

        [Test]
        public void ShouldReturnCorrectChapterData()
        {
            _assert.ShouldMatch(_content, _chapter.BookId);
        }
    }
}
