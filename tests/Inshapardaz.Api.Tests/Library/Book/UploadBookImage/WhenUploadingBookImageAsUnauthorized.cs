using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.UploadBookImage
{
    [TestFixture]
    public class WhenUploadingBookImageAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;
        private BookAssert _assert;
        private int _bookId;
        private byte[] _oldImage;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();
            _bookId = book.Id;
            var imageUrl = BookTestRepository.GetBookImageUrl(_bookId);
            _oldImage = await FileStore.GetFile(imageUrl, CancellationToken.None);

            _response = await Client.PutFile($"/libraries/{LibraryId}/books/{_bookId}/image", RandomData.Bytes);
            _assert = Services.GetService<BookAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorisedResult()
        {
            _response.ShouldBeUnauthorized();
        }

        [Test]
        public void ShouldNotHaveUpdatedBookImage()
        {
            _assert.ShouldNotHaveUpdatedBookImage(_bookId, _oldImage);
        }
    }
}
