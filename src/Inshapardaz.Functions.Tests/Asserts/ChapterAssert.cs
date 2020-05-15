using FluentAssertions;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;

namespace Inshapardaz.Functions.Tests.Asserts
{
    internal class ChapterAssert
    {
        private CreatedResult _response;
        private readonly int _libraryId;
        private ChapterView _chapter;

        public ChapterAssert(CreatedResult response, int libraryId)
        {
            _response = response;
            _libraryId = libraryId;
            _chapter = response.Value as ChapterView;
        }

        internal static ChapterAssert FromResponse(CreatedResult response, int libraryId)
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
            var dbAuthor = dbConnection.GetChapterById(_libraryId, _chapter.Id);
            dbAuthor.Should().NotBeNull();
            _chapter.Title.Should().Be(dbAuthor.Title);
            _chapter.BookId.Should().Be(dbAuthor.BookId);
            _chapter.ChapterNumber.Should().Be(dbAuthor.ChapterNumber);
            return this;
        }

        internal ChapterAssert ShouldHaveSelfLink()
        {
            _chapter.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}");

            return this;
        }

        internal ChapterAssert ShouldHaveBookLink()
        {
            _chapter.Link("book")
                  .ShouldBeGet()
                  .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}");

            return this;
        }

        internal ChapterAssert ShouldHaveUpdateLink()
        {
            _chapter.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}");

            return this;
        }

        internal ChapterAssert ShouldHaveDeleteLink()
        {
            _chapter.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}");

            return this;
        }

        internal ChapterAssert ShouldHaveAddChapterContentLink()
        {
            _chapter.Link("add-contents")
                 .ShouldBePost()
                 .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}/contents");

            return this;
        }

        internal ChapterAssert ShouldHaveUpdateChapterContentLink()
        {
            _chapter.Link("add-contents")
                 .ShouldBePut()
                 .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}/contents");

            return this;
        }

        internal ChapterAssert ShouldHaveDeleteChapterContentLink()
        {
            _chapter.Link("add-contents")
                 .ShouldBeDelete()
                 .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}/contents");

            return this;
        }

        internal ChapterAssert ShouldHaveContentsLink()
        {
            _chapter.Link("contents")
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
    }
}
