using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.GetChapterContents
{
    [TestFixture]
    public class WhenGettingChapterContentLanguageNotExisting
        : LibraryTest<Functions.Library.Books.Chapters.Contents.GetChapterContents>
    {
        private NotFoundResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<ChapterDataBuilder>();
            var chapter = dataBuilder.WithLibrary(LibraryId).Public().WithContents().Build();
            var content = dataBuilder.Contents.Single(x => x.ChapterId == chapter.Id);
            var file = dataBuilder.Files.Single(x => x.Id == content.FileId);
            var fileStore = Container.GetService<IFileStorage>() as FakeFileStorage;
            var contents = fileStore.GetFile(file.FilePath, CancellationToken.None).Result;

            var request = new RequestBuilder()
                .WithAccept(file.MimeType)
                .WithLanguage("some-unknown-language")
                .Build();

            _response = (NotFoundResult)await handler.Run(request, LibraryId, chapter.BookId, chapter.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNotFound()
        {
            _response.ShouldBeNotFound();
        }
    }
}
