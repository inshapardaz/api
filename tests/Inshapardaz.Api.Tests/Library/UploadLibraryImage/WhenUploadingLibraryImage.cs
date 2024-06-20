using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.UploadLibraryImage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenUploadingLibraryImage : TestBase
    {
        private HttpResponseMessage _response;
        private LibraryAssert _assert;
        private byte[] _newImage;

        public WhenUploadingLibraryImage(Role Role)
            : base(Role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _newImage = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{LibraryId}/image", _newImage);
            _assert = Services.GetService<LibraryAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedImage()
        {
            _assert.ShouldHaveUpdatedImage(LibraryId, _newImage);
        }
    }
}
