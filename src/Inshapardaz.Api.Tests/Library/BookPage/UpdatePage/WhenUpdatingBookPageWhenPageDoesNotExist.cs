using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.UpdatePage
{
    [TestFixture]
    public class WhenUpdatingBookPageWhenPageDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private BookPageAssert _assert;
        private BookPageView _page;
        private int _bookId;

        public WhenUpdatingBookPageWhenPageDoesNotExist()
            : base(Domain.Adapters.Permission.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();
            _page = new BookPageView
            {
                BookId = book.Id,
                Text = Random.Text,
                SequenceNumber = Random.Number
            };
            _bookId = book.Id;

            _response = await Client.PutObject($"/library/{LibraryId}/books/{_bookId}/pages/{_page.SequenceNumber}", _page);
            _assert = BookPageAssert.FromResponse(_response, LibraryId);
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
            _assert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldHaveReturnCorrectObject()
        {
            _assert.ShouldMatch(_page);
        }

        [Test]
        public void ShouldHaveSavedBookPage()
        {
            _assert.ShouldHaveSavedPage(DatabaseConnection);
        }
    }
}
