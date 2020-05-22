using FluentAssertions;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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

        public ChapterAssert(ChapterView view, int libraryId)
        {
            _libraryId = libraryId;
            _chapter = view;
        }

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

        internal static ChapterAssert FromObject(ChapterView view, int libraryId)
        {
            return new ChapterAssert(view, libraryId);
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
            var dbChapter = dbConnection.GetChapterById(_chapter.Id);
            dbChapter.Should().NotBeNull();
            _chapter.Title.Should().Be(dbChapter.Title);
            _chapter.BookId.Should().Be(dbChapter.BookId);
            _chapter.ChapterNumber.Should().Be(dbChapter.ChapterNumber);
            return this;
        }

        internal static void ShouldHaveDeletedChapter(int chapterId, IDbConnection databaseConnection)
        {
            var chapter = databaseConnection.GetChapterById(chapterId);
            chapter.Should().BeNull();
        }

        internal static void ThatFilesAreDeletedForChapter(int chapterId, IDbConnection databaseConnection)
        {
            var contents = databaseConnection.GetContentByChapter(chapterId);
            contents.Should().BeNullOrEmpty();
            var files = databaseConnection.GetFilesByChapter(chapterId);
            files.Should().BeNullOrEmpty();
        }

        internal ChapterAssert ShouldHaveSelfLink()
        {
            _chapter.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}");

            return this;
        }

        internal ChapterAssert WithReadOnlyLinks()
        {
            ShouldNotHaveAddChapterContentLink();
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            return this;
        }

        internal ChapterAssert WithWriteableLinks()
        {
            ShouldHaveAddChapterContentLink();
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            return this;
        }

        internal void ShouldHaveContentLink(ChapterContentDto content)
        {
            var actual = _chapter.Contents.Single(x => x.Id == content.Id);
            actual.SelfLink()
                  .ShouldBeGet()
                  .ShouldHaveAcceptLanguage(content.Language);
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

            contents.Should().HaveSameCount(_chapter.Contents);

            foreach (var content in contents)
            {
                ShouldHaveContentLink(content);
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

        internal ChapterAssert ShouldHaveUpdateChapterContentLink(ChapterContentDto content)
        {
            var actual = _chapter.Contents.Single(x => x.Id == content.Id);
            actual.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}/contents");

            return this;
        }

        internal ChapterAssert ShouldHaveDeleteChapterContentLink(ChapterContentDto content)
        {
            var actual = _chapter.Contents.Single(x => x.Id == content.Id);
            actual.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"library/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}/contents");

            return this;
        }

        internal ChapterAssert ShouldNotHaveContentsLink()
        {
            _chapter.Link("content").Should().BeNull();
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

        internal ChapterAssert ShouldBeSameAs(ChapterDto dto)
        {
            _chapter.Title.Should().Be(dto.Title);
            _chapter.ChapterNumber.Should().Be(dto.ChapterNumber);
            _chapter.BookId.Should().Be(dto.BookId);

            return this;
        }
    }

    internal static class ChapterAssertExtensions
    {
        internal static ChapterAssert ShouldMatch(this ChapterView view, ChapterDto dto, int libraryId)
        {
            return ChapterAssert.FromObject(view, libraryId).ShouldBeSameAs(dto);
        }
    }
}
