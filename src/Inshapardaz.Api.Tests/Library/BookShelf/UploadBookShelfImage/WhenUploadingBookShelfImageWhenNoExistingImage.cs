using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.UploadBookShelfImage
{
    [TestFixture]
    public class WhenUploadingBookShelfImageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
        private int _bookShelfId;

        public WhenUploadingBookShelfImageWhenNoExistingImage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var bookShelfId = BookShelfBuilder.WithLibrary(LibraryId).ForAccount(AccountId).WithoutImage().Build();
            _bookShelfId = bookShelfId.Id;

            _response = await Client.PutFile($"/libraries/{LibraryId}/bookshelves/{bookShelfId.Id}/image", RandomData.Bytes);
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
            var bookShelfAssert = BookShelfAssert.WithResponse(_response).InLibrary(LibraryId);
            bookShelfAssert.ShouldHaveCorrectImageLocationHeader(_bookShelfId);
        }

        [Test]
        public void ShouldHaveAddedImageToBookShelf()
        {
            BookShelfAssert.ShouldHaveAddedBookShelfImage(_bookShelfId, DatabaseConnection, FileStore);
        }

        [Test]
        public void ShouldSavePublicImage()
        {
            BookShelfAssert.ShouldHavePublicImage(_bookShelfId, DatabaseConnection);
        }
    }
}
