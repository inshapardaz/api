using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.UpdateChapter
{
    public class WhenUpdatingChapterWithDifferentChapterNumber
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterView _newChapter;
        private ChapterAssert _assert;
        private int _chapterNumber;

        public WhenUpdatingChapterWithDifferentChapterNumber()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapters = ChapterBuilder.WithLibrary(LibraryId).Build(4);
            var chapter = chapters.PickRandom();

            _newChapter = new ChapterView { Title = RandomData.Name, BookId = chapter.BookId };
            _chapterNumber = chapter.ChapterNumber;
            _response = await Client.PutObject($"/libraries/{LibraryId}/books/{chapter.BookId}/chapters/{_chapterNumber}", _newChapter);
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
        public void ShouldHaveUpdatedChater()
        {
            _assert.ShouldHaveSavedChapter(DatabaseConnection);
        }
    }
}
