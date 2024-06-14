using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.UploadBookShelfImage
{
    [TestFixture]
    public class WhenUploadingBookShelfImageAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;
        private int _booKShelfId;
        private byte[] _newImage;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.As(Domain.Models.Role.Reader).Build();
            var bookShelf = BookShelfBuilder.WithLibrary(LibraryId).ForAccount(account.Id).Build();
            _booKShelfId = bookShelf.Id;
            _newImage = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{LibraryId}/bookshelves/{_booKShelfId}/image", _newImage);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }

        [Test]
        public void ShouldNotHaveUpdatedBookShelfImage()
        {
            BookShelfAssert.ShouldNotHaveUpdatedBookShelfImage(_booKShelfId, _newImage, DatabaseConnection, FileStore);
        }
    }
}
