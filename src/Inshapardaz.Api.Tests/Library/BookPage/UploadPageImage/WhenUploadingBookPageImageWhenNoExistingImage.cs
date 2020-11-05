using FluentAssertions;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.UploadPageImage
{
    [TestFixture]
    public class WhenUploadingBookPageImageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
        private BookPageDto _page;
        private int _bookId;

        public WhenUploadingBookPageImageWhenNoExistingImage()
            : base(Domain.Adapters.Permission.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3).Build();
            _page = BookBuilder.GetPages(book.Id).PickRandom();
            _bookId = book.Id;
            _response = await Client.PutFile($"/library/{LibraryId}/books/{_bookId}/pages/{_page.PageNumber}/image", Random.Bytes);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResponse()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            BookPageAssert.ShouldHaveCorrectImageLocationHeader(_response, LibraryId, _bookId, _page.PageNumber);
        }

        [Test]
        public void ShouldHaveAddedImageToBook()
        {
            BookPageAssert.ShouldHaveAddedBookPageImage(_bookId, _page.PageNumber, DatabaseConnection, FileStore);
        }
    }
}
