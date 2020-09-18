using Bogus;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.UpdateBookContent
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenUpdatingBookContentWithPermissions
        : TestBase
    {
        private HttpResponseMessage _response;

        private BookDto _book;
        private BookContentDto _file;
        private byte[] _expected;
        private BookContentAssert _assert;

        public WhenUpdatingBookContentWithPermissions(Permission permission)
            : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).WithContents(2).Build();
            _file = BookBuilder.Contents.PickRandom();

            _expected = new Faker().Image.Random.Bytes(50);

            _response = await Client.PutFile($"/library/{LibraryId}/books/{_book.Id}/content", _expected, _file.Language, _file.MimeType);
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
            _assert.ShouldHaveBookContent(_expected, DatabaseConnection, FileStore);
        }
    }
}
