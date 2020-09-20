using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.UpdateChapter
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenUpdatingChapterWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterView newChapter;
        private ChapterAssert _assert;

        public WhenUpdatingChapterWithPermission(Permission permission)
            : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapters = ChapterBuilder.WithLibrary(LibraryId).Build(4);
            var chapter = chapters.PickRandom();

            newChapter = new ChapterView { Title = Random.Name, ChapterNumber = Random.Number, BookId = chapter.BookId };

            _response = await Client.PutObject($"/library/{LibraryId}/books/{chapter.BookId}/chapters/{chapter.Id}", newChapter);
            _assert = ChapterAssert.FromResponse(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOKResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveReturnedUpdatedChapter()
        {
            _assert.ShouldMatch(newChapter);
        }

        [Test]
        public void ShouldHaveUpdatedChater()
        {
            _assert.ShouldHaveSavedChapter(DatabaseConnection);
        }
    }
}
