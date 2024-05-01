using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.DeleteBookContent
{
    [TestFixture]
    public class WhenDeletingBookContentAsReader
        : TestBase
    {
        private HttpResponseMessage _response;
        private BookContentDto _expected;

        public WhenDeletingBookContentAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithContent().Build();
            _expected = BookBuilder.Contents.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{book.Id}/contents", _expected.Language, _expected.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnForbidden()
        {
            _response.ShouldBeForbidden();
        }

        [Test]
        public void ShouldNotDeletedBookFile()
        {
            BookContentAssert.ShouldHaveBookContent(_expected.BookId, _expected.Language, _expected.MimeType, DatabaseConnection);
        }
    }
}