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
    public class WhenDeletingChapterContentThatDoesNotExists
        : LibraryTest<Functions.Library.Books.Chapters.Contents.DeleteChapterContents>
    {
        private NoContentResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var _dataBuilder = Container.GetService<ChapterDataBuilder>();
            var _newContents = new Faker().Random.Words(12);
            var chapter = _dataBuilder.WithLibrary(LibraryId).WithoutContents().Build();
            var request = new RequestBuilder().WithBody(_newContents)
                                              .Build();
            _response = (NoContentResult)await handler.Run(request, LibraryId, chapter.BookId, chapter.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContentResult()
        {
            _response.ShouldBeNoContent();
        }
    }
}
