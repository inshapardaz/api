using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.BookPage.GetBookPages
{
    [TestFixture]
    public class WhenGettingBookPagesLastPage : TestBase
    {
        private BookDto _book;
        private HttpResponseMessage _response;
        private PagingAssert<BookPageView> _assert;

        public WhenGettingBookPagesLastPage()
            : base(Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).WithPages(23).Build();

            _response = await Client.GetAsync($"/library/{LibraryId}/books/{_book.Id}/pages?pageSize=10&pageNumber=3");

            _assert = new PagingAssert<BookPageView>(_response, Library);
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
            _assert.ShouldHaveSelfLink($"/library/{LibraryId}/books/{_book.Id}/pages");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldNotHaveNextLink()
        {
            _assert.ShouldNotHaveNextLink();
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/library/{LibraryId}/books/{_book.Id}/pages", 2, 10);
        }

        [Test]
        public void ShouldReturExpectedBookPages()
        {
            var expectedItems = BookBuilder.GetPages(_book.Id).OrderBy(p => p.SequenceNumber).Skip(20).Take(10);
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
