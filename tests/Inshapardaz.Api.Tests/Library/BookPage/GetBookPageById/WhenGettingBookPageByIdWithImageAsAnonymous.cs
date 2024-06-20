using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.GetBookPageById
{
    [TestFixture]
    public class WhenGettingBookPageByIdWithImageAsAnonymous
        : TestBase
    {
        private HttpResponseMessage _response;

        private BookPageDto _expected;

        private BookPageAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3, true).Build();
            _expected = BookBuilder.GetPages(book.Id).PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_expected.BookId}/pages/{_expected.SequenceNumber}");
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
                   .ShouldHaveImageLink(_expected.ImageId.Value);
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
