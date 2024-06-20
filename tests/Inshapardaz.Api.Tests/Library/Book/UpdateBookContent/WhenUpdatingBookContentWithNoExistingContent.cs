using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
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
        private string _fileName;
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
            _fileName = RandomData.FileName(_mimeType);

            _book = BookBuilder.WithLibrary(LibraryId).Build();
            _contents = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{LibraryId}/books/{_book.Id}/contents/{-RandomData.Number}?language={_locale}", _contents, _mimeType, _fileName);
            _assert = Services.GetService<BookContentAssert>().ForResponse(_response).ForLibrary(Library);
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
            _assert.ShouldHaveBookContent(_contents, _fileName);
        }
    }
}
