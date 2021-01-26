using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.DeleteChapter
{
    [TestFixture]
    public class WhenDeletingChapterAsReader
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenDeletingChapterAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapter = ChapterBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{chapter.BookId}/chapters/{chapter.ChapterNumber}");
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
