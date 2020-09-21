using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.AddChapterContents
{
    [TestFixture]
    public class WhenAddingChapterContentsForMissingBook
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingChapterContentsForMissingBook()
            : base(Domain.Adapters.Permission.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.PostContent($"/library/{LibraryId}/books/{-Random.Number}/chapters/{Random.Number}/contents", Random.Bytes, Random.Locale, Random.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
