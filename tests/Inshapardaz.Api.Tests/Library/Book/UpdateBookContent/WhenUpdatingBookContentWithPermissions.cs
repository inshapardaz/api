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
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingBookContentWithPermissions
        : TestBase
    {
        private HttpResponseMessage _response;

        private BookDto _book;
        private BookContentDto _file;
        private byte[] _expected;
        private BookContentAssert _assert;
        private string _fileName;

        public WhenUpdatingBookContentWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).WithContents(2).Build();
            _file = BookBuilder.Contents.PickRandom();

            _expected = new Faker().Image.Random.Bytes(50);
            _fileName = RandomData.FileName(_file.MimeType);
            _response = await Client.PutFile($"/libraries/{LibraryId}/books/{_book.Id}/contents/{_file.Id}?language={_file.Language}", _expected, _file.MimeType, _fileName);
            _assert = new BookContentAssert(_response, LibraryId);
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
        public void ShouldHaveUpdatedFileContents()
        {
            _assert.ShouldHaveBookContent(_expected, _fileName, DatabaseConnection, FileStore);
        }
    }
}
