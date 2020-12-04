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
    public class WhenUpdatingBookContentWithDifferentContentType
        : TestBase
    {
        private HttpResponseMessage _response;
        private string _newMimeType;
        private BookDto _book;
        private BookContentDto _file;
        private byte[] _contents;
        private BookContentAssert _assert;

        public WhenUpdatingBookContentWithDifferentContentType()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _newMimeType = "text/markdown";

            _book = BookBuilder.WithLibrary(LibraryId).WithContents(2, "application/pdf").Build();
            _file = BookBuilder.Contents.PickRandom();

            _contents = Random.Bytes;

            _response = await Client.PutFile($"/library/{LibraryId}/books/{_book.Id}/contents", _contents, _file.Language, _newMimeType);
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
            _assert.ShouldHaveCorrectLanguage(_file.Language);
        }

        [Test]
        public void ShouldHaveCorrectMimeType()
        {
            _assert.ShouldHaveCorrectMimeType(_newMimeType);
        }

        [Test]
        public void ShouldHaceCorrectContentSaved()
        {
            _assert.ShouldHaveBookContent(_contents, DatabaseConnection, FileStore);
        }
    }
}
