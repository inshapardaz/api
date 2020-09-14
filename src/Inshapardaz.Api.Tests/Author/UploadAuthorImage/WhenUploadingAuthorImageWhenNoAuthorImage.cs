using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.UploadAuthorImage
{
    [TestFixture]
    public class WhenUploadingAuthorImageWhenNoAuthorImage : TestBase
    {
        private HttpResponseMessage _response;
        private int _authorId;

        public WhenUploadingAuthorImageWhenNoAuthorImage()
            : base(Permission.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = AuthorBuilder.WithLibrary(LibraryId).WithoutImage().Build();
            _authorId = author.Id;

            var newimage = Random.Bytes;

            _response = await Client.PutFile($"/library/{LibraryId}/authors/{_authorId}/image", newimage);
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
            AuthorAssert.ShouldHaveAddedAuthorImage(_authorId, DatabaseConnection, FileStore);
        }

        [Test]
        public void ShouldSavePublicImage()
        {
            AuthorAssert.ShouldHavePublicImage(_authorId, DatabaseConnection);
        }
    }
}
