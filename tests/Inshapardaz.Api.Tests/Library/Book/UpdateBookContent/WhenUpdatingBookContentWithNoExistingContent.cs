using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.UpdateBookContent
{
    [TestFixture]
    public class WhenUpdatingBookContentWithNoExistingContent
        : TestBase
    {
        private HttpResponseMessage _response;
        private BookContentAssert _assert;
        private BookDto _book;
        private string _mimeType;
        private string _locale;
        private byte[] _contents;

        public WhenUpdatingBookContentWithNoExistingContent()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _mimeType = RandomData.MimeType;
            _locale = RandomData.Locale;

            _book = BookBuilder.WithLibrary(LibraryId).Build();
            _contents = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{LibraryId}/books/{_book.Id}/contents/{-RandomData.Number}?language={_locale}", _contents, _mimeType, "test.md");
            _assert = new BookContentAssert(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveCorrectLink()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHaveBookLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveCorrectLanguage()
        {
            _assert.ShouldHaveCorrectLanguage(_locale);
        }

        [Test]
        public void ShouldHaveCorrectMimeType()
        {
            _assert.ShouldHaveCorrectMimeType(_mimeType);
        }

        [Test]
        public void ShouldHaceCorrectContentSaved()
        {
            _assert.ShouldHaveBookContent(_contents, DatabaseConnection, FileStore);
        }
    }
}
