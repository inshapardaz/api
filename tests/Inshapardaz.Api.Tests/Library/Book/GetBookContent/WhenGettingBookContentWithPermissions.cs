using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.GetBookContent
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenGettingBookContentWithPermissions
        : TestBase
    {
        private HttpResponseMessage _response;
        private BookContentAssert _assert;
        private BookDto _book;
        private BookContentDto _expected;

        public WhenGettingBookContentWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).WithContents(5).Build();
            _expected = BookBuilder.Contents.PickRandom();
            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_book.Id}/contents/{_expected.Id}?language={_expected.Language}", _expected.MimeType);
            _assert = Services.GetService<BookContentAssert>().ForResponse(_response).ForLibrary(Library);
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
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldHaveEditLinks()
        {
            _assert.ShouldHaveUpdateLink();
            _assert.ShouldHaveDeleteLink();
        }

        [Test]
        public void SHouldHaveDownloadLink()
        {
            _assert.ShouldHavePrivateDownloadLink();
        }

        [Test]
        public void ShouldHaveCorrectMimeType()
        {
            _assert.ShouldHaveCorrectMimeType(_expected.MimeType);
        }

        [Test]
        public void ShouldHaveCorrectLanguage()
        {
            _assert.ShouldHaveCorrectLanguage(_expected.Language);
        }

        [Test]
        public void ShouldReturnCorrectChapterData()
        {
            _assert.ShouldMatch(_expected, _book.Id);
        }
    }
}
