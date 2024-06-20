using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.UploadLibraryImage
{
    [TestFixture]
    public class WhenUploadingLibraryImageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
        private LibraryAssert _assert;
        private int _libraryId;

        private byte[] _newImage;

        public WhenUploadingLibraryImageWhenNoExistingImage()
            : base(Role.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var library = LibraryBuilder.WithoutImage().Build();
            _libraryId = library.Id;
            _newImage = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{_libraryId}/image", _newImage);
            _assert = Services.GetService<LibraryAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveAddedImageToLibrary()
        {
            _assert.ShouldHaveAddedLibraryImage(_libraryId, _newImage);
        }

        [Test]
        public void ShouldSavePublicImage()
        {
            _assert.ShouldHavePublicImage(_libraryId);
        }
    }
}
