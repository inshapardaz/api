using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.UpdateChapter
{
    [TestFixture]
    public class WhenUpdatingChapterThatDoesNotExist
        : LibraryTest<Functions.Library.Books.Chapters.UpdateChapter>
    {
        private CreatedResult _response;
        private ChapterView _newChapter;
        private ChapterAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var bookBuilder = Container.GetService<BooksDataBuilder>();
            var book = bookBuilder.WithLibrary(LibraryId).Build();

            _newChapter = new ChapterView { Title = Random.Name, BookId = book.Id, ChapterNumber = Random.Number };
            _response = (CreatedResult)await handler.Run(_newChapter, LibraryId, book.Id, _newChapter.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
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
            _assert.ShouldHaveCorrectLoactionHeader();
        }

        [Test]
        public void ShouldSaveTheChapter()
        {
            _assert.ShouldHaveSavedChapter(DatabaseConnection);
        }

        [Test]
        public void ShouldHaveCorrectObjectRetured()
        {
            _assert.ShouldMatch(_newChapter);
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
