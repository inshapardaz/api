using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.UpdatePage
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenUpdatingBookPageWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private BookPageAssert _assert;
        private BookPageDto _page;
        private BookPageView _updatedPage;
        private int _bookId;

        public WhenUpdatingBookPageWithPermissions(Permission permission)
            : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3, true).Build();
            _page = BookBuilder.GetPages(book.Id).PickRandom();

            _updatedPage = new BookPageView
            {
                BookId = book.Id,
                Text = Random.Text,
                SequenceNumber = _page.SequenceNumber
            };

            _bookId = book.Id;
            _response = await Client.PutObject($"/library/{LibraryId}/books/{_bookId}/pages/{_page.SequenceNumber}", _updatedPage);
            _assert = BookPageAssert.FromResponse(_response, LibraryId);
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
        public void ShouldHaveReturnCorrectObject()
        {
            _assert.ShouldMatch(_updatedPage);
        }

        [Test]
        public void ShouldHaveSavedBookPage()
        {
            _assert.ShouldHaveSavedPage(DatabaseConnection);
        }
    }
}
