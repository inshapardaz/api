using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.UploadAuthorImage
{
    [TestFixture]
    public class WhenUploadingAuthorImageAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;
        private int _authorId;
        private byte[] _oldImage;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = AuthorBuilder.WithLibrary(LibraryId).Build();
            _authorId = author.Id;
            var imageUrl = DatabaseConnection.GetAuthorImageUrl(_authorId);
            _oldImage = await FileStore.GetFile(imageUrl, CancellationToken.None);

            var newimage = Random.Bytes;
            var client = CreateClient();
            _response = await client.PutFile($"/library/{LibraryId}/authors/{_authorId}/image", newimage);
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
        public void ShouldNotHaveUpdatedAuthorImage()
        {
            AuthorAssert.ShouldNotHaveUpdatedAuthorImage(_authorId, _oldImage, DatabaseConnection, FileStore);
        }
    }
}
