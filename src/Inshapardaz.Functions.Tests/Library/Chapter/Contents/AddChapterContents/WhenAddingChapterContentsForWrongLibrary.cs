using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.AddChapterContents
{
    [TestFixture]
    public class WhenAddingChapterContentsForWrongLibrary
        : LibraryTest<Functions.Library.Books.Chapters.Contents.AddChapterContents>
    {
        private BadRequestResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = Container.GetService<BooksDataBuilder>().WithLibrary(LibraryId).Build();
            var chapter = Container.GetService<ChapterDataBuilder>().WithLibrary(LibraryId).Build();
            var request = new RequestBuilder().WithContentType("something").WithBody("test content").Build();
            _response = (BadRequestResult)await handler.Run(request, -Random.Number, book.Id, chapter.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
