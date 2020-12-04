using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.GetBookPageById
{
    [TestFixture]
    public class WhenGettingBookPageByIdAsAnonymous
        : TestBase
    {
        private HttpResponseMessage _response;

        private BookPageDto _expected;

        private BookPageAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3).Build();
            _expected = BookBuilder.GetPages(book.Id).PickRandom();

            _response = await Client.GetAsync($"/library/{LibraryId}/books/{_expected.BookId}/pages/{_expected.SequenceNumber}");
            _assert = BookPageAssert.FromResponse(_response, LibraryId);
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
        public void ShouldNotHaveEditLinks()
        {
            _assert.ShouldNotHaveUpdateLink()
                   .ShouldNotHaveDeleteLink()
                   .ShouldNotHaveImageUpdateLink()
                   .ShouldNotHaveImageDeleteLink();
        }
    }
}
