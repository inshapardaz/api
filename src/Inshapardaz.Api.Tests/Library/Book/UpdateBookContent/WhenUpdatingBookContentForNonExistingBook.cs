using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.UpdateBookContent
{
    [TestFixture]
    public class WhenUpdatingBookContentForNonExistingBook
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUpdatingBookContentForNonExistingBook() : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var newContents = Random.Bytes;

            _response = await Client.PutFile($"/libraries/{LibraryId}/books/{-Random.Number}/contents", newContents, Random.Locale, Random.MimeType);
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