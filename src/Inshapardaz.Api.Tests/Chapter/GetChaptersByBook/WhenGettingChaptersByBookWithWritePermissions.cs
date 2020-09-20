using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.GetChaptersByBook
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenGettingChaptersByBookWithWritePermissions
        : TestBase
    {
        private HttpResponseMessage _response;
        private ListView<ChapterView> _view;
        private BookDto _book;

        public WhenGettingChaptersByBookWithWritePermissions(Permission permission)
            : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapters = ChapterBuilder.WithLibrary(LibraryId).WithContents().Build(4);
            _book = DatabaseConnection.GetBookById(chapters.PickRandom().BookId);

            _response = await Client.GetAsync($"/library/{LibraryId}/books/{_book.Id}/chapters");
            _view = _response.GetContent<ListView<ChapterView>>().Result;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.SelfLink()
                .ShouldBeGet()
                .EndingWith($"library/{LibraryId}/books/{_book.Id}/chapters");
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            _view.CreateLink()
                .ShouldBePost()
                .EndingWith($"library/{LibraryId}/books/{_book.Id}/chapters");
        }

        [Test]
        public void ShouldHaveCorrectNumberOfChapters()
        {
            Assert.That(_view.Data.Count(), Is.EqualTo(4));
        }

        [Test]
        public void ShouldHaveCorrectChaptersData()
        {
            foreach (var expected in ChapterBuilder.Chapters.Where(c => c.BookId == _book.Id))
            {
                var actual = _view.Data.FirstOrDefault(x => x.Id == expected.Id);
                var assert = new ChapterAssert(actual, LibraryId);
                assert.ShouldBeSameAs(expected)
                      .WithWriteableLinks();

                var contents = ChapterBuilder.Contents.Where(c => c.ChapterId == expected.Id);
                foreach (var content in contents)
                {
                    assert.ShouldHaveContentLink(content);
                    assert.ShouldHaveUpdateChapterContentLink(content);
                    assert.ShouldHaveDeleteChapterContentLink(content);
                }
            }
        }
    }
}
