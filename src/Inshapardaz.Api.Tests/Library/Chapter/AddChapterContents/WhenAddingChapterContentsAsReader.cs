using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.AddChapterContents
{
    [TestFixture]
    public class WhenAddingChapterContentsAsReader
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingChapterContentsAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapter = ChapterBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.PostContent($"/library/{LibraryId}/books/{chapter.BookId}/chapters/{chapter.Id}/contents", Random.Bytes, Random.Locale, Random.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}
