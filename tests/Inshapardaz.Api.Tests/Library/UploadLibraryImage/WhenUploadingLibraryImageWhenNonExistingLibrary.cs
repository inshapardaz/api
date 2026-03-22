using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.UploadLibraryImage
{
    [TestFixture]
    public class WhenUploadingLibraryImageWhenNonExistingLibrary() : TestBase(Role.LibraryAdmin)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var newimage = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{-RandomData.Number}/image", newimage);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveReturnedForbidden() => _response.ShouldBeForbidden();
    }
}
