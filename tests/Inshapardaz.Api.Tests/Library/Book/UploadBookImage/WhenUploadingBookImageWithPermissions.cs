using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.UploadBookImage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUploadingBookImageWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private BookAssert _assert;
        private int _bookId;
        private byte[] _newImage = RandomData.Bytes;

        public WhenUploadingBookImageWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();
            _bookId = book.Id;

            var imageUrl = BookTestRepository.GetBookImageUrl(_bookId);

            _response = await Client.PutFile($"/libraries/{LibraryId}/books/{_bookId}/image", _newImage);
            _assert = Services.GetService<BookAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _assert.ShouldHaveUpdatedBookImage(_bookId, _newImage);
        }
    }
}
