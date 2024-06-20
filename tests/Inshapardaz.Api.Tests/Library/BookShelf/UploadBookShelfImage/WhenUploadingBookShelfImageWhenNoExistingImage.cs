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
    public class WhenUploadingBookShelfImageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
        private BookShelfAssert _assert;
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
            _assert = Services.GetService<BookShelfAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _assert.ShouldHaveCorrectImageLocationHeader(_bookShelfId);
        }

        [Test]
        public void ShouldHaveAddedImageToBookShelf()
        {
            _assert.ShouldHaveAddedBookShelfImage(_bookShelfId);
        }

        [Test]
        public void ShouldSavePublicImage()
        {
            _assert.ShouldHavePublicImage(_bookShelfId);
        }
    }
}
