using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.UploadBookImage
{
    [TestFixture]
    public class WhenUploadingBookImageWhenNoExistingImage() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private BookAssert _assert;
        private int _bookId;
        private byte[] _newImage = RandomData.Bytes;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithNoImage().Build();
            _bookId = book.Id;

            _response = await Client.PutFile($"/libraries/{LibraryId}/books/{_bookId}/image", _newImage);
            _assert = Services.GetService<BookAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveHttpResponseMessage() => _response.ShouldBeCreated();

        [Test]
        public void ShouldHaveLocationHeader() => _assert.ShouldHaveCorrectImageLocationHeader(_bookId);

        [Test]
        public void ShouldHaveAddedImageToBook() => _assert.ShouldHaveAddedBookImage(_bookId);

        [Test]
        public void ShouldReturnChecksum() => _assert.ShouldHaveChecksum(_newImage);
    }
}
