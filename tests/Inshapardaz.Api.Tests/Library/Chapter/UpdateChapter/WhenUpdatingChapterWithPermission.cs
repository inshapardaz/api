using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.UpdateChapter
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingChapterWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterView newChapter;
        private ChapterAssert _assert;

        public WhenUpdatingChapterWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapters = ChapterBuilder.WithLibrary(LibraryId).Build(4);
            var chapter = chapters.PickRandom();

            newChapter = new ChapterView { Title = RandomData.Name, BookId = chapter.BookId };

            _response = await Client.PutObject($"/libraries/{LibraryId}/books/{chapter.BookId}/chapters/{chapter.ChapterNumber}", newChapter);
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
