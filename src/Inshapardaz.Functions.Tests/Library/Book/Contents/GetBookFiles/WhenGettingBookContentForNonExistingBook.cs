using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Contents.GetBookFile
{
    [TestFixture]
    public class WhenGettingBookContentForNonExistingBook
        : LibraryTest<Functions.Library.Books.Content.GetBookContent>
    {
        private StatusCodeResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = new RequestBuilder().WithAccept(MimeTypes.Pdf).Build();
            _response = (StatusCodeResult)await handler.Run(request, LibraryId, Random.Number, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNotFoundResult()
        {
            _response.ShouldBeNotFound();
        }
    }
}
