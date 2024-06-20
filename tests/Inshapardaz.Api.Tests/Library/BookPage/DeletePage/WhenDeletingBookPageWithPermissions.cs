using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
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
        private BookPageAssert _assert;
        private BookPageDto _page;
        private string _filePath;
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
            _filePath = FileTestRepository.GetFileById(_page.ContentId.Value)?.FilePath;

            _bookId = book.Id;
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{_bookId}/pages/{_page.SequenceNumber}");
            _assert = Services.GetService<BookPageAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _assert.ShouldHaveNoBookPage(_bookId, _page.Id, _page.ImageId);
        }


        [Test]
        public void ShouldHaveDeletedTheContentFile()
        {
            _assert.ShouldHaveNoBookPageContent(_page.ContentId.Value, _filePath);
        }
    }
}
