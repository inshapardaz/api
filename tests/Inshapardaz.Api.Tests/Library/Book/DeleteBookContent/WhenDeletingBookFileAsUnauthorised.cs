using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.DeleteBookContent
{
    [TestFixture]
    public class WhenDeletingBookFileAsUnauthorised
        : TestBase
    {
        private HttpResponseMessage _response;

        private BookContentDto _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithContent().Build();
            _expected = BookBuilder.Contents.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{book.Id}/contents/{_expected.Id}?language={_expected.Language}", _expected.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            _response.ShouldBeUnauthorized();
        }

        [Test]
        public void ShouldNotDeletedBookFile()
        {
            BookContentAssert.ShouldHaveBookContent(_expected.BookId, _expected.Language, _expected.MimeType, DatabaseConnection);
        }
    }
}
