using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.UploadAuthorImage
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenUploadingAuthorImageWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private int _authorId;
        private byte[] _oldImage;

        public WhenUploadingAuthorImageWithPermissions(Permission Permission)
            : base(Permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = AuthorBuilder.WithLibrary(LibraryId).Build();
            _authorId = author.Id;

            var imageUrl = DatabaseConnection.GetAuthorImageUrl(_authorId);

            _oldImage = await FileStore.GetFile(imageUrl, CancellationToken.None);
            var newimage = Random.Bytes;

            _response = await Client.PutFile($"/library/{LibraryId}/authors/{_authorId}/image", newimage);
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
        public void ShouldHaveUpdatedAuthorImage()
        {
            AuthorAssert.ShouldHaveUpdatedAuthorImage(_authorId, _oldImage, DatabaseConnection, FileStore);
        }
    }
}
