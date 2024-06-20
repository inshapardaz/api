using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.UploadBookShelfImage
{
    [TestFixture]
    public class WhenUploadingBookShelfImageAsOtherUser : TestBase
    {
        private HttpResponseMessage _response;
        private BookShelfAssert _assert;
        private int _bookShelfId;
        private byte[] _newImage;

        public WhenUploadingBookShelfImageAsOtherUser()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Build();
            var bookshelf = BookShelfBuilder.WithLibrary(LibraryId).ForAccount(account.Id).Build();
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
        public void ShouldHaveForbidResult()
        {
            _response.ShouldBeForbidden();
        }

        [Test]
        public void ShouldNotHaveUpdatedBookShelfImage()
        {
            _assert.ShouldNotHaveUpdatedBookShelfImage(_bookShelfId, _newImage);
        }
    }
}
