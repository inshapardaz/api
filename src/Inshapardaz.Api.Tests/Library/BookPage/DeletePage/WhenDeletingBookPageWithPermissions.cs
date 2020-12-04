using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.DeletePage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingBookPageWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private BookPageDto _page;
        private int _bookId;

        public WhenDeletingBookPageWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3, true).Build();
            _page = BookBuilder.GetPages(book.Id).PickRandom();
            _bookId = book.Id;
            _response = await Client.DeleteAsync($"/library/{LibraryId}/books/{_bookId}/pages/{_page.SequenceNumber}");
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
        public void ShouldDeletePage()
        {
            BookPageAssert.ShouldHaveNoBookPage(_bookId, _page.SequenceNumber, _page.ImageId, DatabaseConnection, FileStore);
        }
    }
}
