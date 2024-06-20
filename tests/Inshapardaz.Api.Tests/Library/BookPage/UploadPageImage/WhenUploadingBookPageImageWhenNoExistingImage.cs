using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.UploadPageImage
{
    [TestFixture]
    public class WhenUploadingBookPageImageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
        private BookPageAssert _assert;
        private BookPageDto _page;
        private int _bookId;

        public WhenUploadingBookPageImageWhenNoExistingImage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3).Build();
            _page = BookBuilder.GetPages(book.Id).PickRandom();
            _bookId = book.Id;
            _response = await Client.PutFile($"/libraries/{LibraryId}/books/{_bookId}/pages/{_page.SequenceNumber}/image", RandomData.Bytes);
            _assert = Services.GetService<BookPageAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            var savedPage = BookPageTestRepository.GetBookPageByNumber(_page.BookId, _page.SequenceNumber);
            _assert.ShouldHaveCorrectImageLocationHeader(_response, savedPage.ImageId.Value);
        }

        [Test]
        public void ShouldHaveAddedImageToBook()
        {
            _assert.ShouldHaveAddedBookPageImage(_bookId, _page.SequenceNumber);
        }
    }
}
