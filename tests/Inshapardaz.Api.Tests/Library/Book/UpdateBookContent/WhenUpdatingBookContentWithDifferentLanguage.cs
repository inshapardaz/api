using Bogus;
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
    public class WhenUpdatingBookContentWithDifferentLanguage
        : TestBase
    {
        private HttpResponseMessage _response;
        private string _newLanguage = RandomData.Locale;
        private string _fileName;
        private BookDto _book;
        private BookContentDto _content;
        private byte[] _contents;
        private BookContentAssert _assert;

        public WhenUpdatingBookContentWithDifferentLanguage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).WithContents(2).WithContentLanguage($"{_newLanguage}_old").Build();
            _content = BookBuilder.Contents.PickRandom();

            _contents = new Faker().Image.Random.Bytes(50);
            _fileName = RandomData.FileName(_content.MimeType);

            _response = await Client.PutFile($"/libraries/{LibraryId}/books/{_book.Id}/contents/{_content.Id}?language={_newLanguage}", _contents, _content.MimeType, _fileName);
            _assert = Services.GetService<BookContentAssert>().ForResponse(_response).ForLibrary(Library);
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
            _assert.ShouldHaveCorrectLanguage(_newLanguage);
        }

        [Test]
        public void ShouldHaveCorrectMimeType()
        {
            _assert.ShouldHaveCorrectMimeType(_content.MimeType);
        }

        [Test]
        public void ShouldHaceCorrectContentSaved()
        {
            _assert.ShouldHaveBookContent(_contents, _fileName);
        }
    }
}
