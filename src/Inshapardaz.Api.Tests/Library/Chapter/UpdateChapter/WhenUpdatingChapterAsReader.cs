using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.UpdateChapter
{
    [TestFixture]
    public class WhenUpdatingChapterAsReader
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUpdatingChapterAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapters = ChapterBuilder.WithLibrary(LibraryId).WithContents().Build(4);
            var chapter = chapters.PickRandom();

            var chapter2 = new ChapterView { Title = Random.Name };

            _response = await Client.PutObject($"/libraries/{LibraryId}/books/{chapter.BookId}/chapters/{chapter.ChapterNumber}", chapter2);
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
