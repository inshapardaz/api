using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.UpdateChapterContents
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingChapterContentsWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterDto _chapter;
        private ChapterContentDto _content;
        private ChapterContentAssert _assert;

        private byte[] _newContents;

        public WhenUpdatingChapterContentsWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _chapter = ChapterBuilder.WithLibrary(LibraryId).Public().WithContents().Build();
            _content = ChapterBuilder.Contents.Single(x => x.ChapterId == _chapter.Id);
            var file = ChapterBuilder.Files.Single(x => x.Id == _content.FileId);
            var contents = FileStore.GetFile(file.FilePath, CancellationToken.None).Result;

            _newContents = Random.Bytes;

            _response = await Client.PutContent($"/libraries/{LibraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/contents", _newContents, _content.Language, file.MimeType);
            _assert = new ChapterContentAssert(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHaveBookLink()
                   .ShouldHaveChapterLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHavePublicDownloadLink()
                   .ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveUpdatedContents()
        {
            _assert.ShouldHaveCorrectContents(_newContents, FileStore, DatabaseConnection);
        }
    }
}
