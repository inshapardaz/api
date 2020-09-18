using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.DeleteBookContent
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenDeletingBookFileWithPermissions
        : TestBase
    {
        private HttpResponseMessage _response;
        private BookContentDto _expected;

        public WhenDeletingBookFileWithPermissions(Permission permission)
            : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithContents(3).Build();
            _expected = BookBuilder.Contents.PickRandom();

            _response = await Client.DeleteAsync($"/library/{LibraryId}/books/{book.Id}/contents", _expected.Language, _expected.MimeType);
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
            BookContentAssert.ShouldNotHaveBookContent(_expected.BookId, _expected.Language, _expected.MimeType, DatabaseConnection);
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

                BookContentAssert.ShouldHaveBookContent(item.BookId, item.Language, item.MimeType, DatabaseConnection);
            }
        }
    }
}
