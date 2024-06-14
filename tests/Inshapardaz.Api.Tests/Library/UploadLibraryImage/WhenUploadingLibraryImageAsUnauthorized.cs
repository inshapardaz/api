using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.UploadLibraryImage
{
    [TestFixture]
    public class WhenUploadingLibraryImageAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;
        private byte[] _newImage;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _newImage = RandomData.Bytes;
            _response = await Client.PutFile($"/libraries/{LibraryId}/image", _newImage);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }

        [Test]
        public void ShouldNotHaveUpdatedLibraryImage()
        {
            LibraryAssert.ShouldNotHaveUpdatedLibraryImage(LibraryId, _newImage, DatabaseConnection, FileStore);
        }
    }
}
