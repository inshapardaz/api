using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.GetChapterContents
{
    [TestFixture]
    public class WhenGettingChapterContentWhenNoChapterNotExists
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenGettingChapterContentWhenNoChapterNotExists()
            : base(Domain.Adapters.Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.GetAsync($"/library/{LibraryId}/books/{book.Id}/chapters/{-Random.Number}/contents", Random.Locale, Random.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNotFoundResult()
        {
            _response.ShouldBeNotFound();
        }
    }
}
