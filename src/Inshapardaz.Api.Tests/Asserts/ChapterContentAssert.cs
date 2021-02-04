using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Asserts
{
    internal class ChapterContentAssert
    {
        private HttpResponseMessage _response;
        private readonly int _libraryId;
        private ChapterContentView _chapterContent;
        private LibraryDto _library;

        public ChapterContentAssert(HttpResponseMessage response, int libraryId)
        {
            _response = response;
            _libraryId = libraryId;
            _chapterContent = response.GetContent<ChapterContentView>().Result;
        }

        public ChapterContentAssert(HttpResponseMessage response, LibraryDto library)
        {
            _response = response;
            _libraryId = library.Id;
            _library = library;
            _chapterContent = response.GetContent<ChapterContentView>().Result;
        }

        internal ChapterContentAssert ShouldHaveSelfLink()
        {
            _chapterContent.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterNumber}/contents")
                  .ShouldHaveAcceptLanguage(_chapterContent.Language);

            return this;
        }

        internal ChapterContentAssert WithReadOnlyLinks()
        {
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            return this;
        }

        internal ChapterContentAssert WithWriteableLinks()
        {
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            return this;
        }

        internal ChapterContentAssert ShouldHaveUpdateLink()
        {
            _chapterContent.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"libraries/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterNumber}/contents")
                 .ShouldHaveAcceptLanguage(_chapterContent.Language);

            return this;
        }

        internal ChapterContentAssert ShouldHaveText(string contents)
        {
            _chapterContent.Text.Should().Be(contents);
            return this;
        }

        internal ChapterContentAssert ShouldNotHaveUpdateLink()
        {
            _chapterContent.UpdateLink().Should().BeNull();
            return this;
        }

        internal ChapterContentAssert ShouldHaveDefaultLibraryLanguage()
        {
            _chapterContent.Language.Should().Be(_library.Language);
            return this;
        }

        internal ChapterContentAssert ShouldHaveCorrectLoactionHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"libraries/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterNumber}/contents");
            return this;
        }

        internal ChapterContentAssert ShouldHaveSavedCorrectText(string expected, IDbConnection dbConnection)
        {
            var content = dbConnection.GetChapterContentById(_chapterContent.Id);
            content.Text.Should().NotBeNull().And.Be(expected);
            return this;
        }

        internal ChapterContentAssert ShouldHaveMatechingTextForLanguage(string expected, string language, IDbConnection dbConnection)
        {
            var content = dbConnection.GetChapterContentById(_chapterContent.Id);
            content.Text.Should().NotBeNull().And.Should().NotBe(expected);
            content.Language.Should().Be(language);
            return this;
        }

        internal ChapterContentAssert ShouldHaveContentLink()
        {
            _chapterContent.Link("content")
                           .ShouldBeGet();

            return this;
        }

        internal ChapterContentAssert ShouldHaveSavedChapterContent(IDbConnection dbConnection)
        {
            var dbContent = dbConnection.GetChapterContentById(_chapterContent.Id);
            dbContent.Should().NotBeNull();
            _chapterContent.ChapterId.Should().Be(dbContent.ChapterId);
            _chapterContent.Language.Should().Be(dbContent.Language);

            return this;
        }

        internal ChapterContentAssert ShouldHaveDeleteLink()
        {
            _chapterContent.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"libraries/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterNumber}/contents");

            return this;
        }

        internal ChapterContentAssert ShouldNotHaveDeleteLink()
        {
            _chapterContent.DeleteLink().Should().BeNull();
            return this;
        }

        internal ChapterContentAssert ShouldHaveChapterLink()
        {
            _chapterContent.Link("chapter")
                .ShouldBeGet()
                .EndingWith($"libraries/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterNumber}");

            return this;
        }

        internal ChapterContentAssert ShouldHaveBookLink()
        {
            _chapterContent.Link("book")
                .ShouldBeGet()
                .EndingWith($"libraries/{_libraryId}/books/{_chapterContent.BookId}");

            return this;
        }

        internal ChapterContentAssert ShouldMatch(ChapterContentDto content, int bookId, IDbConnection dbConnection)
        {
            _chapterContent.ChapterId.Should().Be(content.ChapterId);
            _chapterContent.BookId.Should().Be(bookId);
            _chapterContent.Language.Should().Be(content.Language);

            return this;
        }

        internal static void ShouldHaveDeletedContent(IDbConnection dbConnection, ChapterContentDto content)
        {
            var dbContent = dbConnection.GetChapterContentById(content.Id);
            dbContent.Should().BeNull("Chapter contnet should be deleted");
        }

        internal static void ShouldHaveLocationHeader(RedirectResult result, int libraryId, int bookId, ChapterContentDto content)
        {
            var response = result as RedirectResult;
            response.Url.Should().NotBeNull();
            response.Url.Should().EndWith($"libraries/{libraryId}/books/{bookId}/chapters/{content.ChapterId}/contents/{content.Id}");
        }
    }
}
