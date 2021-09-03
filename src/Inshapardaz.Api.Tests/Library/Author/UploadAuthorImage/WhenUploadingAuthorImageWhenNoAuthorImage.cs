using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.UploadAuthorImage
{
    [TestFixture]
    public class WhenUploadingAuthorImageWhenNoAuthorImage : TestBase
    {
        private HttpResponseMessage _response;
        private int _authorId;

        private byte[] _newImage;

        public WhenUploadingAuthorImageWhenNoAuthorImage()
            : base(Role.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = AuthorBuilder.WithLibrary(LibraryId).WithoutImage().Build();
            _authorId = author.Id;
            _newImage = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{LibraryId}/authors/{_authorId}/image", _newImage);
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
        public void ShouldHaveLocationHeader()
        {
            var authorAssert = AuthorAssert.WithResponse(_response).InLibrary(LibraryId);
            authorAssert.ShouldHaveCorrectImageLocationHeader(_authorId);
        }

        [Test]
        public void ShouldHaveAddedImageToAuthor()
        {
            AuthorAssert.ShouldHaveAddedAuthorImage(_authorId, _newImage, DatabaseConnection, FileStore);
        }

        [Test]
        public void ShouldSavePublicImage()
        {
            AuthorAssert.ShouldHavePublicImage(_authorId, DatabaseConnection);
        }
    }
}
