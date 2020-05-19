using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.AddChapterContents
{
    [TestFixture]
    public class WhenAddingChapterContentsForMissingBook
        : LibraryTest<Functions.Library.Books.Chapters.Contents.AddChapterContents>
    {
        private BadRequestResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = new RequestBuilder().WithBody("test content").Build();
            _response = (BadRequestResult)await handler.Run(request, LibraryId, Random.Number, Random.Number, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
