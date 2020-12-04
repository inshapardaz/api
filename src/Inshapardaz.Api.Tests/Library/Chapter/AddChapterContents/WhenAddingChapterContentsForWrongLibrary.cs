using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.AddChapterContents
{
    [TestFixture]
    public class WhenAddingChapterContentsForWrongLibrary
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingChapterContentsForWrongLibrary()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();
            var chapter = ChapterBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.PostContent($"/library/{-Random.Number}/books/{book.Id}/chapters/{chapter.Id}/contents", Random.Bytes, Random.Locale, Random.MimeType);
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
