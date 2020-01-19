using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.AddChapterContents
{
    [TestFixture]
    public class WhenAddingChapterContentsForMissingChapter : FunctionTest
    {
        private BadRequestResult _response;
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<BooksDataBuilder>();
            var book = dataBuilder.Build();
            var handler = Container.GetService<Functions.Library.Books.Chapters.Contents.AddChapterContents>();
            var request = new RequestBuilder().WithBody("test content").Build();
            _response = (BadRequestResult) await handler.Run(request, book.Id, Random.Number, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
