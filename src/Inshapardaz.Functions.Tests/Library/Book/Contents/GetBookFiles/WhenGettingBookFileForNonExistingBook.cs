using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Contents.GetBookFile
{
    [TestFixture, Ignore("ToFix")]
    public class WhenGettingBookFileForNonExistingBook
        : LibraryTest<Functions.Library.Books.Content.GetBookContent>
    {
        private NotFoundResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = new RequestBuilder().Build();
            _response = (NotFoundResult)await handler.Run(request, LibraryId, Random.Number, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNotFoundResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
