using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.UploadPageImage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUploadingBookPageImageWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private BookPageDto _page;
        private int _bookId;
        private byte[] _newImage = RandomData.Bytes;

        public WhenUploadingBookPageImageWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3, true).Build();
            _page = BookBuilder.GetPages(book.Id).PickRandom();
            _bookId = book.Id;
            _response = await Client.PutFile($"/libraries/{LibraryId}/books/{_bookId}/pages/{_page.SequenceNumber}/image", _newImage);
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
        public void ShouldHaveUpdatedBookPageImage()
        {
            BookPageAssert.ShouldHaveUpdatedBookPageImage(_bookId, _page.SequenceNumber, _newImage, DatabaseConnection, FileStore);
        }
    }
}
