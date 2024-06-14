using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
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
            var newContents = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{LibraryId}/books/{-RandomData.Number}/contents/{-RandomData.Number}?language={RandomData.Locale}", newContents, RandomData.MimeType);
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
