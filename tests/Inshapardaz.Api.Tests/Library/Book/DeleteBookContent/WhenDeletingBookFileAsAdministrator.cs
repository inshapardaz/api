using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.DeleteBookContent
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingBookFileWithPermissions
        : TestBase
    {
        private HttpResponseMessage _response;
        private BookContentAssert _assert;
        private BookContentDto _expected;

        public WhenDeletingBookFileWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithContents(3).Build();
            _expected = BookBuilder.Contents.PickRandom();
            //_filePath = DatabaseConnection.GetFileById(_expected.FileId).FilePath;
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{book.Id}/contents/{_expected.Id}?language={_expected.Language}", _expected.MimeType);
            _assert = Services.GetService<BookContentAssert>().ForResponse(_response).ForLibrary(Library);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNoContentResult()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedBookFile()
        {
            _assert.ShouldNotHaveBookContent(_expected.BookId, _expected.Language, _expected.MimeType);
            FileAssert.FileDoesnotExist(_expected.FileId, _expected.FilePath);
        }

        [Test]
        public void ShouldNotHaveDeletedOtherBookFiles()
        {
            foreach (var item in BookBuilder.Contents)
            {
                if (item.Id == _expected.Id)
                {
                    continue;
                }

                _assert.ShouldHaveBookContent(item.BookId, item.Language, item.MimeType);
            }
        }
    }
}
