using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.UploadLibraryImage
{

    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenUploadingLibraryImageAsNonAdmin : TestBase
    {
        private HttpResponseMessage _response;
        private byte[] _newImage;

        public WhenUploadingLibraryImageAsNonAdmin(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {;
            _newImage = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{LibraryId}/image", _newImage);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbidResult()
        {
            _response.ShouldBeForbidden();
        }

        [Test]
        public void ShouldNotHaveUpdatedLibraryImage()
        {
            LibraryAssert.ShouldNotHaveUpdatedLibraryImage(LibraryId, _newImage, DatabaseConnection, FileStore);
        }
    }
}
