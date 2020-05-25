using System.Linq;
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

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.GetChapterContents
{
    [TestFixture]
    public class WhenGettingPrivateChapterContentAsUnauthorised
        : LibraryTest<Functions.Library.Books.Chapters.Contents.GetChapterContents>
    {
        private UnauthorizedResult _response;
        private DefaultHttpRequest _request;
        private ChapterDto _chapter;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<ChapterDataBuilder>();
            _chapter = dataBuilder.WithLibrary(LibraryId).Private().WithContents().Build();
            var content = dataBuilder.Contents.Single(x => x.ChapterId == _chapter.Id);
            var file = dataBuilder.Files.Single(x => x.Id == content.FileId);
            var fileStore = Container.GetService<IFileStorage>() as FakeFileStorage;
            var contents = fileStore.GetFile(file.FilePath, CancellationToken.None).Result;

            _request = new RequestBuilder()
                            .WithAccept(file.MimeType)
                            .WithLanguage(content.Language)
                            .Build();

            _response = (UnauthorizedResult)await handler.Run(_request, LibraryId, _chapter.BookId, _chapter.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
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
