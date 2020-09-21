using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.UploadBookImage
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenUploadingBookImageWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private int _bookId;
        private byte[] _newImage = Random.Bytes;

        public WhenUploadingBookImageWithPermissions(Permission permission)
            : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();
            _bookId = book.Id;

            var imageUrl = DatabaseConnection.GetBookImageUrl(_bookId);

            _response = await Client.PutFile($"/library/{LibraryId}/books/{_bookId}/image", _newImage);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedBookImage()
        {
            BookAssert.ShouldHaveUpdatedBookImage(_bookId, _newImage, DatabaseConnection, FileStore);
        }
    }
}
