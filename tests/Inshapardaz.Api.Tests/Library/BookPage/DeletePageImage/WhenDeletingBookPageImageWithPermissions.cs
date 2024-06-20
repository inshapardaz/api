using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.DeletePageImage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingBookPageImageWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private BookPageAssert _assert;
        private BookPageDto _page;
        private int _bookId;

        public WhenDeletingBookPageImageWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3, true).Build();
            _page = BookBuilder.GetPages(book.Id).PickRandom();
            _bookId = book.Id;
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{_bookId}/pages/{_page.SequenceNumber}/image");
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
        public void ShouldHaveDeletedPageImage()
        {
            _assert.ShouldHaveNoBookPageImage(_bookId, _page.SequenceNumber, _page.ImageId.Value);
        }
    }
}
