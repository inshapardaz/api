using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.UploadLibraryImage
{
    [TestFixture]
    public class WhenUploadingLibraryImageWhenNonExistingLibrary : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUploadingLibraryImageWhenNonExistingLibrary()
            : base(Role.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var newimage = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{-RandomData.Number}/image", newimage);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveReturnedForbidden()
        {
            _response.ShouldBeForbidden();
        }
    }
}
