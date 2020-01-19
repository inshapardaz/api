using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.DeleteChapterContents
{
    [TestFixture]
    public class WhenDeletingChapterContentThatDoesNotExists : FunctionTest
    {
        private NoContentResult _response;
        private ChapterDataBuilder _dataBuilder;


        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<ChapterDataBuilder>();

            var chapter = _dataBuilder.AsPublic().Build();
            var handler = Container.GetService<Functions.Library.Books.Chapters.Contents.DeleteChapterContents>();
            _response = (NoContentResult) await handler.Run(null, chapter.BookId, chapter.Id, -Random.Number,  AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNoContentResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
