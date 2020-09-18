using Bogus;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.UpdateBookContent
{
    [TestFixture]
    public class WhenUpdatingBookContentAsReader
        : TestBase
    {
        private HttpResponseMessage _response;
        private BookDto _book;
        private byte[] _expected;

        public WhenUpdatingBookContentAsReader()
            : base(Domain.Adapters.Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).WithContent().Build();
            var file = BookBuilder.Contents.PickRandom();
            _expected = new Faker().Image.Random.Bytes(50);

            _response = await Client.PutFile($"/library/{LibraryId}/books/{_book.Id}/content", _expected, file.Language, file.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnForbidResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}
