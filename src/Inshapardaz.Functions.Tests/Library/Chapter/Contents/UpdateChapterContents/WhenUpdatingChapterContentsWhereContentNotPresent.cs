using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.UpdateChapterContents
{
    [TestFixture]
    public class WhenUpdatingChapterContentsWhereContentNotPresent
        : LibraryTest<Functions.Library.Books.Chapters.Contents.UpdateChapterContents>
    {
        private ObjectResult _response;
        private ChapterDto _chapter;
        private ChapterContentAssert _assert;
        private FakeFileStorage _fileStore;
        private DefaultHttpRequest _request;
        private byte[] _newContents;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<ChapterDataBuilder>();
            _fileStore = Container.GetService<IFileStorage>() as FakeFileStorage;
            _chapter = dataBuilder.WithLibrary(LibraryId).Public().WithContents().Build();

            _newContents = Random.Bytes;

            _request = new RequestBuilder()
                                .WithContentType("text/markdown")
                                .WithLanguage("en")
                                .WithBytes(_newContents)
                                .Build();

            _response = (CreatedResult)await handler.Run(_request, LibraryId, _chapter.BookId, _chapter.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);

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
        public void ShouldHaveLocationHeader()
        {
            _assert.ShouldHaveCorrectLoactionHeader();
        }

        [Test]
        public void ShouldSaveTheChapterContent()
        {
            _assert.ShouldHaveSavedChapterContent(DatabaseConnection);
        }

        [Test]
        public void ShouldHaveCorrectObjectReturened()
        {
            _assert.ShouldMatch(_request, _chapter);
        }

        [Test]
        public void ShouldHaveCorrectContentSaved()
        {
            _assert.ShouldHaveCorrectContents(_newContents, _fileStore, DatabaseConnection);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHaveBookLink()
                   .ShouldHaveChapterLink()
                   .ShouldHavePublicDownloadLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink();
        }
    }
}
