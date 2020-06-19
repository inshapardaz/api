using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.UpdateChapterContents
{
    [TestFixture]
    public class WhenUpdatingChapterContentsWithDifferentLanguage
        : LibraryTest<Functions.Library.Books.Chapters.Contents.UpdateChapterContents>
    {
        private ObjectResult _response;
        private ChapterDto _chapter;
        private ChapterContentDto _content;
        private ChapterContentAssert _assert;
        private FakeFileStorage _fileStore;

        private byte[] _newContents;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<ChapterDataBuilder>();
            _chapter = dataBuilder.WithLibrary(LibraryId).Public().WithContents().Build();
            _content = dataBuilder.Contents.Single(x => x.ChapterId == _chapter.Id);
            var file = dataBuilder.Files.Single(x => x.Id == _content.FileId);
            _fileStore = Container.GetService<IFileStorage>() as FakeFileStorage;
            var contents = _fileStore.GetFile(file.FilePath, CancellationToken.None).Result;

            _newContents = Random.Bytes;

            var request = new RequestBuilder()
                                .WithContentType(file.MimeType)
                                .WithLanguage(_content.Language + "1")
                                .WithBytes(_newContents)
                                .Build();

            _response = (ObjectResult)await handler.Run(request, LibraryId, _chapter.BookId, _chapter.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);

            _assert = new ChapterContentAssert(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
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
            _assert.ShouldHaveCorrectContentsForLanguage(_newContents, _content.Language + "1", _fileStore, DatabaseConnection);
        }
    }
}
