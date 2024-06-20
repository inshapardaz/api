using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.GetBookPageById
{
    [TestFixture]
    public class WhenGettingLastPageOfBook
        : TestBase
    {
        private HttpResponseMessage _response;

        private BookPageDto _expected;

        private BookPageAssert _assert;

        public WhenGettingLastPageOfBook() : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3).Build();
            _expected = BookBuilder.GetPages(book.Id).Last();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{book.Id}/pages/{_expected.SequenceNumber}");
            _assert = Services.GetService<BookPageAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveCorrectObjectRetured()
        {
            _assert.ShouldMatch(_expected);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHaveBookLink()
                   .ShouldNotHaveImageLink();
        }

        [Test]
        public void ShouldHavePreviousLinks()
        {
            _assert.ShouldHavePreviousLinkForPageNumber(_expected.SequenceNumber - 1);
        }

        [Test]
        public void ShouldNotHaveNoNextLink()
        {
            _assert.ShouldHaveNoNextLink();
        }
    }
}
