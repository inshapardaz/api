using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
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

            _response = await Client.PutFile($"/libraries/{LibraryId}/books/{_book.Id}/contents/{-RandomData.Number}?language={file.Language}", _expected, file.MimeType);
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
