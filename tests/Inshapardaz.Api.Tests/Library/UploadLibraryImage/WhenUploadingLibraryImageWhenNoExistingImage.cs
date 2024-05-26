using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.UploadLibraryImage
{
    [TestFixture]
    public class WhenUploadingLibraryImageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
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
            LibraryAssert.ShouldHaveAddedLibraryImage(_libraryId, _newImage, DatabaseConnection, FileStore);
        }

        [Test]
        public void ShouldSavePublicImage()
        {
            LibraryAssert.ShouldHavePublicImage(_libraryId, DatabaseConnection);
        }
    }
}
