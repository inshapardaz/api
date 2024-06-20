using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.UpdatePage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingBookPageWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private BookPageAssert _assert;
        private BookPageDto _page;
        private BookPageView _updatedPage;
        private int _bookId;
        private string _text;

        public WhenUpdatingBookPageWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3, true).Build();
            _page = BookBuilder.GetPages(book.Id).PickRandom();

            _text = RandomData.Text;
            _updatedPage = new BookPageView
            {
                BookId = book.Id,
                Text = _text,
                SequenceNumber = _page.SequenceNumber
            };

            _bookId = book.Id;
            _response = await Client.PutObject($"/libraries/{LibraryId}/books/{_bookId}/pages/{_page.SequenceNumber}", _updatedPage);
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
        public void ShouldHaveReturnCorrectObject()
        {
            _assert.ShouldMatch(_updatedPage);
        }

        [Test]
        public void ShouldHaveSavedBookPage()
        {
            _assert.ShouldHaveSavedPage();
        }

        [Test]
        public void ShouldHaveSavedContents()
        {
            _assert.ShouldHaveBookPageContent(_text);
        }
    }
}
