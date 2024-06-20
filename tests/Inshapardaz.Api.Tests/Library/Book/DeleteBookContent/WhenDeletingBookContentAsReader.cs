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
    [TestFixture]
    public class WhenDeletingBookContentAsReader
        : TestBase
    {
        private HttpResponseMessage _response;
        private BookContentAssert _assert;
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

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{book.Id}/contents/{_expected.Id}?language={_expected.Language}", _expected.MimeType);
            _assert = Services.GetService<BookContentAssert>().ForResponse(_response).ForLibrary(Library);
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
            _assert.ShouldHaveBookContent(_expected.BookId, _expected.Language, _expected.MimeType);
        }
    }
}
