using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using System.Data;
using System.Linq;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Asserts
{
    internal class ChapterAssert
    {
        private HttpResponseMessage _response;
        private readonly int _libraryId;
        private ChapterView _chapter;

        public ChapterAssert(ChapterView view, int libraryId)
        {
            _libraryId = libraryId;
            _chapter = view;
        }

        public ChapterAssert(HttpResponseMessage response, int libraryId)
        {
            _response = response;
            _libraryId = libraryId;
            _chapter = response.GetContent<ChapterView>().Result;
        }

        internal static ChapterAssert FromResponse(HttpResponseMessage response, int libraryId)
        {
            return new ChapterAssert(response, libraryId);
        }

        internal static ChapterAssert FromObject(ChapterView view, int libraryId)
        {
            return new ChapterAssert(view, libraryId);
        }

        internal ChapterAssert ShouldHaveCorrectLoactionHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"libraries/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}");
            return this;
        }

        internal ChapterAssert ShouldHaveSavedChapter(IDbConnection dbConnection)
        {
            var dbChapter = dbConnection.GetChapterByBookAndChapter(_chapter.BookId, _chapter.Id);
            dbChapter.Should().NotBeNull();
            _chapter.Title.Should().Be(dbChapter.Title);
            _chapter.BookId.Should().Be(dbChapter.BookId);
            return this;
        }

        internal static void ShouldHaveDeletedChapter(int chapterId, IDbConnection databaseConnection)
        {
            var chapter = databaseConnection.GetChapterById(chapterId);
            chapter.Should().BeNull();
        }

        internal static void ThatContentsAreDeletedForChapter(int chapterId, IDbConnection databaseConnection)
        {
            var contents = databaseConnection.GetContentByChapter(chapterId);
            contents.Should().BeNullOrEmpty();
        }

        internal ChapterAssert ShouldHaveSelfLink()
        {
            _chapter.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}");

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
                  .EndingWith($"libraries/{_libraryId}/books/{_chapter.BookId}");

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
                 .EndingWith($"libraries/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}");

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
                 .EndingWith($"libraries/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}");

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
                 .EndingWith($"libraries/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/contents");

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
                  .EndingWith($"libraries/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/contents")
                  .ShouldHaveAcceptLanguage(content.Language);

            return this;
        }

        internal ChapterAssert ShouldHaveDeleteChapterContentLink(ChapterContentDto content)
        {
            var actual = _chapter.Contents.Single(x => x.Id == content.Id);
            actual.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/contents")
                  .ShouldHaveAcceptLanguage(actual.Language);

            return this;
        }

        internal ChapterAssert ShouldNotHaveContentsLink()
        {
            _chapter.Link("content").Should().BeNull();
            return this;
        }

        internal ChapterAssert ShouldHaveNotNextLink()
        {
            _chapter.Link("next").Should().BeNull();
            return this;
        }

        internal ChapterAssert ShouldHaveNextLink(int chapterNumber)
        {
            _chapter.Link("next")
                    .ShouldBeGet()
                    .EndingWith($"/chapters/{chapterNumber}");
            return this;
        }

        internal ChapterAssert ShouldHaveNotPreviousLink()
        {
            _chapter.Link("previous").Should().BeNull();
            return this;
        }

        internal ChapterAssert ShouldHavePreviousLink(int chapterNumber)
        {
            _chapter.Link("previous")
                    .ShouldBeGet()
                    .EndingWith($"/chapters/{chapterNumber}");
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
