using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.AddChapter
{
    [TestFixture]
    public class WhenAddingChapterWithoutChapterNumber
        : TestBase
    {
        private ChapterView _chapter;
        private HttpResponseMessage _response;
        private ChapterAssert _assert;
        private BookDto _book;

        public WhenAddingChapterWithoutChapterNumber()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).WithChapters(3).Build();

            _chapter = new ChapterView { Title = RandomData.Name };

            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{_book.Id}/chapters", _chapter);

            _assert = ChapterAssert.FromResponse(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _assert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldSaveTheChapter()
        {
            _assert.ShouldHaveSavedChapter(DatabaseConnection);
        }

        [Test]
        public void ShouldHaveCorrectObjectRetured()
        {
            var expected = new ChapterView { Title = _chapter.Title, BookId = _book.Id, ChapterNumber = 4 };
            _assert.ShouldMatch(expected);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHaveBookLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink()
                   .ShouldNotHaveContentsLink()
                   .ShouldHaveAddChapterContentLink();
        }
    }
}
