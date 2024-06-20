using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.AddBookContent
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAddingBookContentWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private readonly string _mimeType = RandomData.MimeType;
        private readonly string _locale = RandomData.Locale;
        private string _fileName;
        private BookContentAssert _assert;
        private byte[] _contents;

        public WhenAddingBookContentWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _fileName = RandomData.FileName(_mimeType);
            var book = BookBuilder.WithLibrary(LibraryId).Build();
            _contents = RandomData.Bytes;

            _response = await Client.PostContent($"/libraries/{LibraryId}/books/{book.Id}/contents?language={_locale}", _contents, _mimeType, _fileName);

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
                   .ShouldHavePrivateDownloadLink()
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
