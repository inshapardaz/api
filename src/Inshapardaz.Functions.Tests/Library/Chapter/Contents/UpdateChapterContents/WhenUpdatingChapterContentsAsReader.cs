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
    public class WhenUpdatingChapterContentsAsReader
        : LibraryTest<Functions.Library.Books.Chapters.Contents.UpdateChapterContents>
    {
        private ForbidResult _response;
        private ChapterDto _chapter;
        private ChapterContentDto _content;

        private byte[] _newContents;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<ChapterDataBuilder>();
            _chapter = dataBuilder.WithLibrary(LibraryId).Public().WithContents().Build();
            _content = dataBuilder.Contents.Single(x => x.ChapterId == _chapter.Id);
            var file = dataBuilder.Files.Single(x => x.Id == _content.FileId);
            var fileStore = Container.GetService<IFileStorage>() as FakeFileStorage;
            var contents = fileStore.GetFile(file.FilePath, CancellationToken.None).Result;

            _newContents = Random.Bytes;

            var request = new RequestBuilder()
                                .WithContentType(file.MimeType)
                                .WithLanguage(_content.Language)
                                .WithBytes(_newContents)
                                .Build();
            _response = (ForbidResult)await handler.Run(request, LibraryId, _chapter.BookId, _chapter.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
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
