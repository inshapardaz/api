using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.UpdateBookContent
{
    [TestFixture]
    public class WhenUpdatingBookContentForNonExistingBook
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUpdatingBookContentForNonExistingBook() : base(Domain.Adapters.Permission.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var newContents = Random.Bytes;

            _response = await Client.PutFile($"/library/{LibraryId}/books/{-Random.Number}/content", newContents, Random.Locale, Random.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
