using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.UploadAuthorImage
{
    [TestFixture]
    public class WhenUploadingAuthorImageWhenNonExistingAuthor : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUploadingAuthorImageWhenNonExistingAuthor()
            : base(Role.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var newimage = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{LibraryId}/authors/{-RandomData.Number}/image", newimage);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequest()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
