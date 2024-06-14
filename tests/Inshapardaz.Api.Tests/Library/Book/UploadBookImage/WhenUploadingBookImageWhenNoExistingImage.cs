using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.UploadBookImage
{
    [TestFixture]
    public class WhenUploadingBookImageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
        private BookAssert _bookAssert;
        private int _bookId;

        public WhenUploadingBookImageWhenNoExistingImage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithNoImage().Build();
            _bookId = book.Id;

            _response = await Client.PutFile($"/libraries/{LibraryId}/books/{_bookId}/image", RandomData.Bytes);
            _bookAssert = BookAssert.WithResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveHttpResponseMessage()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _bookAssert.ShouldHaveCorrectImageLocationHeader(_bookId);
        }

        [Test]
        public void ShouldHaveAddedImageToBook()
        {
            BookAssert.ShouldHaveAddedBookImage(_bookId, DatabaseConnection, FileStore);
        }

        [Test]
        public void ShouldHavePublicImage()
        {
            BookAssert.ShouldHavePublicImage(_bookId, DatabaseConnection);
        }
    }
}
