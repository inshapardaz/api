using Bogus;
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
    public class WhenUpdatingBookContentWithDifferentLanguage
        : TestBase
    {
        private HttpResponseMessage _response;
        private string _newLanguage;
        private BookDto _book;
        private BookContentDto _file;
        private byte[] _contents;
        private BookContentAssert _assert;

        public WhenUpdatingBookContentWithDifferentLanguage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _newLanguage = Random.Locale;

            _book = BookBuilder.WithLibrary(LibraryId).WithContents(2).WithContentLanguage($"{_newLanguage}_old").Build();
            _file = BookBuilder.Contents.PickRandom();

            _contents = new Faker().Image.Random.Bytes(50);

            _response = await Client.PutFile($"/libraries/{LibraryId}/books/{_book.Id}/contents", _contents, _newLanguage, _file.MimeType);
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
            _assert.ShouldHaveCorrectLanguage(_newLanguage);
        }

        [Test]
        public void ShouldHaveCorrectMimeType()
        {
            _assert.ShouldHaveCorrectMimeType(_file.MimeType);
        }

        [Test]
        public void ShouldHaceCorrectContentSaved()
        {
            _assert.ShouldHaveBookContent(_contents, DatabaseConnection, FileStore);
        }
    }
}