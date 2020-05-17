using FluentAssertions;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Linq;

namespace Inshapardaz.Functions.Tests.Asserts
{
    internal class ChapterAssert
    {
        private ObjectResult _response;
        private readonly int _libraryId;
        private ChapterView _chapter;

        public ChapterAssert(ObjectResult response, int libraryId)
        {
            _response = response;
            _libraryId = libraryId;
            _chapter = response.Value as ChapterView;
        }

        internal static ChapterAssert FromResponse(ObjectResult response, int libraryId)
        {
            return new ChapterAssert(response, libraryId);
        }

        internal ChapterAssert ShouldHaveCorrectLoactionHeader()
        {
            var response = _response as CreatedResult;
            response.Location.Should().NotBeNull();
            response.Location.Should().EndWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}");
            return this;
        }

        internal ChapterAssert ShouldHaveSavedChapter(IDbConnection dbConnection)
        {
            var dbAuthor = dbConnection.GetChapterById(_chapter.Id);
            dbAuthor.Should().NotBeNull();
            _chapter.Title.Should().Be(dbAuthor.Title);
            _chapter.BookId.Should().Be(dbAuthor.BookId);
            _chapter.ChapterNumber.Should().Be(dbAuthor.ChapterNumber);
            return this;
        }

        internal static void ShouldHaveDeletedChapter(int chapterId, IDbConnection databaseConnection)
        {
            var chapter = databaseConnection.GetChapterById(chapterId);
            chapter.Should().BeNull();
        }

        internal static void ThatFilesAreDeletedForChapter(int chapterId, IDbConnection databaseConnection)
        {
            var contents = databaseConnection.GetFilesByChapter(chapterId);
            contents.Should().BeNullOrEmpty();
        }

        internal ChapterAssert ShouldHaveSelfLink()
        {
            _chapter.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}");

            return this;
        }

        internal void ShouldHaveNoCorrectContents()
        {
            _chapter.Link("content").Should().BeNull();
        }

        internal ChapterAssert ShouldHaveBookLink()
        {
            _chapter.Link("book")
                  .ShouldBeGet()
                  .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}");

            return this;
        }

        internal void ShouldHaveCorrectContents(IDbConnection db)
        {
            var contents = db.GetContentByChapter(_chapter.Id);

            contents.Should().HaveSameCount(_chapter.Links("content"));
            foreach (var contentLink in _chapter.Links("content"))
            {
                var expected = contents.SingleOrDefault(x => x.Language == contentLink.AcceptLanguage);
                expected.Should().NotBeNull();
            }
        }

        internal ChapterAssert ShouldHaveUpdateLink()
        {
            _chapter.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}");

            return this;
        }

        internal ChapterAssert ShouldNotHaveUpdateLink()
        {
            _chapter.UpdateLink().Should().BeNull();
            return this;
        }

        internal ChapterAssert ShouldHaveDeleteLink()
        {
            _chapter.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}");

            return this;
        }

        internal ChapterAssert ShouldNotHaveDeleteLink()
        {
            _chapter.DeleteLink().Should().BeNull();
            return this;
        }

        internal ChapterAssert ShouldHaveAddChapterContentLink()
        {
            _chapter.Link("add-content")
                 .ShouldBePost()
                 .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}/contents");

            return this;
        }

        internal ChapterAssert ShouldNotHaveAddChapterContentLink()
        {
            _chapter.Link("add-content").Should().BeNull();
            return this;
        }

        internal ChapterAssert ShouldHaveUpdateChapterContentLink()
        {
            _chapter.Link("add-content")
                 .ShouldBePut()
                 .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}/contents");

            return this;
        }

        internal ChapterAssert ShouldHaveDeleteChapterContentLink()
        {
            _chapter.Link("add-content")
                 .ShouldBeDelete()
                 .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}/contents");

            return this;
        }

        internal ChapterAssert ShouldHaveContentsLink()
        {
            _chapter.Link("content")
                  .ShouldBeGet()
                  .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}/contents");
            return this;
        }

        internal ChapterAssert ShouldNotHaveContentsLink()
        {
            _chapter.Link("contents").Should().BeNull();
            return this;
        }

        internal void ShouldMatch(ChapterView view)
        {
            _chapter.Title.Should().Be(view.Title);
            _chapter.BookId.Should().Be(view.BookId);
            _chapter.ChapterNumber.Should().Be(view.ChapterNumber);
        }

        internal void ShouldMatch(ChapterDto dto)
        {
            _chapter.Title.Should().Be(dto.Title);
            _chapter.BookId.Should().Be(dto.BookId);
            _chapter.ChapterNumber.Should().Be(dto.ChapterNumber);
        }
    }
}
