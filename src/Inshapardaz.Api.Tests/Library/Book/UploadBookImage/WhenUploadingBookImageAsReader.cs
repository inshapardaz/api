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
    [TestFixture]
    public class WhenUploadingBookImageAsReader : TestBase
    {
        private HttpResponseMessage _response;
        private int _bookId;
        private byte[] _oldImage;

        public WhenUploadingBookImageAsReader()
            : base(Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();
            _bookId = book.Id;
            var imageUrl = DatabaseConnection.GetBookImageUrl(_bookId);
            _oldImage = await FileStore.GetFile(imageUrl, CancellationToken.None);

            _response = await Client.PutFile($"/library/{LibraryId}/books/{_bookId}/image", Random.Bytes);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbidResult()
        {
            _response.ShouldBeForbidden();
        }

        [Test]
        public void ShouldNotHaveUpdatedBookImage()
        {
            BookAssert.ShouldNotHaveUpdatedBookImage(_bookId, _oldImage, DatabaseConnection, FileStore);
        }
    }
}
