using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.GetBooksByBookShelf
{
    [TestFixture]
    public class WhenGettingBooksByBookShelfAsUnauthorized
        : TestBase

    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private BookShelfDto _bookShelf;
        private IEnumerable<BookDto> _bookShelfBooks;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Build();
            _bookShelf = BookShelfBuilder.WithLibrary(LibraryId).ForAccount(account.Id).Build();
            _bookShelfBooks = BookBuilder.WithLibrary(LibraryId).InBookShelf(_bookShelf.Id).IsPublic().Build(5);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?pageNumber=1&pageSize=10&bookShelfId={_bookShelf.Id}");

            _assert = Services.GetService<PagingAssert<BookView>>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNotFound()
        {
            _response.ShouldBeNotFound();
        }
    }
}
