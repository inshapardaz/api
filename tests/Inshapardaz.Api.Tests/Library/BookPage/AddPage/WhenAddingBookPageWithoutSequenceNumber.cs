using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.AddPage
{
    public class WhenAddingBookPageWithoutSequenceNumber
        : TestBase
    {
        private BookDto _book;
        private BookPageView _page;
        private HttpResponseMessage _response;
        private BookPageAssert _assert;

        public WhenAddingBookPageWithoutSequenceNumber()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).WithPages().Build();

            _page = new BookPageView { BookId = _book.Id, Text = RandomData.Text };
            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{_book.Id}/pages", _page);

            _assert = BookPageAssert.FromResponse(_response, LibraryId);
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
        public void ShouldSavedThePageWithLastPageOfBook()
        {
            var bookPageCount = DatabaseConnection.GetBookPageCount(_book.Id);
            _assert.ShouldHavePageNumber(bookPageCount);
        }


        [Test]
        public void ShouldHaveSavedTheContentFile()
        {
            _assert.ShouldHaveBookPageContent(_page.Text, DatabaseConnection, FileStore);
        }

    }
}
