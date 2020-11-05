using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.UploadPageImage
{
    [TestFixture]
    public class WhenUploadingBookPageImageAsReader : TestBase
    {
        private HttpResponseMessage _response;
        private BookPageDto _page;
        private int _bookId;

        public WhenUploadingBookPageImageAsReader()
            : base(Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3).Build();
            _page = BookBuilder.GetPages(book.Id).PickRandom();
            _bookId = book.Id;
            _response = await Client.PutFile($"/library/{LibraryId}/books/{_bookId}/pages/{_page.SequenceNumber}/image", Random.Bytes);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbidResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}
