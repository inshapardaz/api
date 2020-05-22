using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.DeleteChapterContents
{
    [TestFixture]
    public class WhenDeletingChapterContentsAsUnauthorized
        : LibraryTest<Functions.Library.Books.Chapters.Contents.UpdateChapterContents>
    {
        private UnauthorizedResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var _dataBuilder = Container.GetService<ChapterDataBuilder>();
            var _newContents = new Faker().Random.Words(12);
            var chapter = _dataBuilder.WithLibrary(LibraryId).WithContents().Build();
            var content = _dataBuilder.Contents.Single(x => x.ChapterId == chapter.Id);
            var file = _dataBuilder.Files.Single(x => x.Id == content.FileId);
            var request = new RequestBuilder().WithBody(_newContents)
                                              .WithLanguage(content.Language)
                                              .WithContentType(file.MimeType)
                                              .Build();
            _response = (UnauthorizedResult)await handler.Run(request, LibraryId, chapter.BookId, chapter.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
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
