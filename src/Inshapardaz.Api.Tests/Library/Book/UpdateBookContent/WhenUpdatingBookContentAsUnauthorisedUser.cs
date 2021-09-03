using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.UpdateBookContent
{
    [TestFixture]
    public class WhenUpdatingBookContentAsUnauthorisedUser
        : TestBase
    {
        private HttpResponseMessage _response;

        private BookDto _book;
        private byte[] _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).WithContent().Build();
            var file = BookBuilder.Contents.PickRandom();

            _expected = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{LibraryId}/books/{_book.Id}/contents", _expected, file.Language, file.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
