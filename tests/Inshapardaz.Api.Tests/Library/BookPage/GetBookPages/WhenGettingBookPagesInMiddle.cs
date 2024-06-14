using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookPage.GetBookPages
{
    [TestFixture]
    public class WhenGettingBookPagesInMiddle : TestBase
    {
        private BookDto _book;
        private HttpResponseMessage _response;
        private PagingAssert<BookPageView> _assert;

        public WhenGettingBookPagesInMiddle()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).WithPages(23).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_book.Id}/pages?pageSize=10&pageNumber=2");

            _assert = new PagingAssert<BookPageView>(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books/{_book.Id}/pages");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/books/{_book.Id}/pages", 3, 10);
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/books/{_book.Id}/pages", 1, 10);
        }

        [Test]
        public void ShouldReturExpectedBookPages()
        {
            var expectedItems = BookBuilder.GetPages(_book.Id).OrderBy(p => p.SequenceNumber).Skip(10).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.SequenceNumber == item.SequenceNumber);
                actual.ShouldMatch(item)
                    .InLibrary(LibraryId)
                            .ShouldHaveSelfLink()
                            .ShouldHaveBookLink()
                            .ShouldNotHaveImageLink()
                            .ShouldNotHaveUpdateLink()
                            .ShouldNotHaveDeleteLink()
                            .ShouldNotHaveImageUpdateLink()
                            .ShouldNotHaveImageDeleteLink();
            }
        }
    }
}
