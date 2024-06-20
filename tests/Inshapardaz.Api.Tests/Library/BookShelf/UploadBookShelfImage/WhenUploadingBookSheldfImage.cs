using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.UploadBookShelfImage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenUploadingBookSheldfImage : TestBase
    {
        private HttpResponseMessage _response;
        private BookShelfAssert _assert;
        private int _bookShelfId;
        private byte[] _newImage;

        public WhenUploadingBookSheldfImage(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var bookshelf = BookShelfBuilder.WithLibrary(LibraryId).ForAccount(AccountId).Build();
            _bookShelfId = bookshelf.Id;
            _newImage = RandomData.Bytes;
            _response = await Client.PutFile($"/libraries/{LibraryId}/bookshelves/{bookshelf.Id}/image", _newImage);
            _assert = Services.GetService<BookShelfAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
        public void ShouldHaveUpdatedBookShelfImage()
        {
            _assert.ShouldHaveUpdatedBookShelfImage(_bookShelfId, _newImage);
        }
    }
}
