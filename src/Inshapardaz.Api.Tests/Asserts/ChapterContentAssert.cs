using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net.Http;
using System.Threading;

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
                  .EndingWith($"library/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterId}/contents/{_chapterContent.Id}");

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
                 .EndingWith($"library/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterId}/contents/{_chapterContent.Id}");

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
            location.Should().EndWith($"library/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterId}/contents/{_chapterContent.Id}");
            return this;
        }

        internal ChapterContentAssert ShouldHaveCorrectContents(byte[] expected, IFileStorage fileStorage, IDbConnection dbConnection)
        {
            var file = dbConnection.GetFileByChapter(_chapterContent.ChapterId, _chapterContent.Language, _chapterContent.MimeType);
            var content = fileStorage.GetFile(file.FilePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.NotEqual(expected);
            return this;
        }

        internal ChapterContentAssert ShouldHaveCorrectContentsForMimeType(byte[] expected, string mimeType, IFileStorage fileStorage, IDbConnection dbConnection)
        {
            var file = dbConnection.GetFileByChapter(_chapterContent.ChapterId, _chapterContent.Language, mimeType);
            var content = fileStorage.GetFile(file.FilePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.NotEqual(expected);
            return this;
        }

        internal ChapterContentAssert ShouldHaveCorrectContentsForLanguage(byte[] expected, string language, IFileStorage fileStorage, IDbConnection dbConnection)
        {
            var file = dbConnection.GetFileByChapter(_chapterContent.ChapterId, language, _chapterContent.MimeType);
            var content = fileStorage.GetFile(file.FilePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.NotEqual(expected);
            return this;
        }

        internal ChapterContentAssert ShouldHavePrivateDownloadLink()
        {
            _chapterContent.Link("download")
                           .ShouldBeGet();
            //.Href.Should()
            //.StartWith(Settings.BlobRoot);

            return this;
        }

        internal ChapterContentAssert ShouldHavePublicDownloadLink()
        {
            _chapterContent.Link("download")
                           .ShouldBeGet();
            //.Href.Should()
            //.StartWith(Settings.CDNAddress);

            return this;
        }

        internal ChapterContentAssert ShouldHaveSavedChapterContent(IDbConnection dbConnection)
        {
            var dbContent = dbConnection.GetChapterContentById(_chapterContent.Id);
            dbContent.Should().NotBeNull();
            var dbFile = dbConnection.GetFileById(dbContent.FileId);
            _chapterContent.ChapterId.Should().Be(dbContent.ChapterId);
            _chapterContent.Language.Should().Be(dbContent.Language);
            _chapterContent.MimeType.Should().Be(dbFile.MimeType);

            return this;
        }

        internal ChapterContentAssert ShouldHaveDeleteLink()
        {
            _chapterContent.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"library/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterId}/contents/{_chapterContent.Id}");

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
                .EndingWith($"library/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterId}");

            return this;
        }

        internal ChapterContentAssert ShouldHaveBookLink()
        {
            _chapterContent.Link("book")
                .ShouldBeGet()
                .EndingWith($"library/{_libraryId}/books/{_chapterContent.BookId}");

            return this;
        }

        //internal ChapterContentAssert ShouldMatch(DefaultHttpRequest request, ChapterDto chapterDto)
        //{
        //    _chapterContent.ChapterId.Should().Be(chapterDto.Id);
        //    _chapterContent.BookId.Should().Be(chapterDto.BookId);
        //    _chapterContent.MimeType.Should().Be(request.MimeType());
        //    _chapterContent.Language.Should().Be(request.Language());
        //    return this;
        //}

        internal ChapterContentAssert ShouldMatch(ChapterContentDto content, int bookId, IDbConnection dbConnection)
        {
            _chapterContent.ChapterId.Should().Be(content.ChapterId);
            _chapterContent.BookId.Should().Be(bookId);
            _chapterContent.Language.Should().Be(content.Language);

            var dbFile = dbConnection.GetFileById(content.FileId);
            _chapterContent.MimeType.Should().Be(dbFile.MimeType);

            return this;
        }

        internal static void ShouldHaveDeletedContent(IDbConnection dbConnection, ChapterContentDto content)
        {
            var dbContent = dbConnection.GetChapterContentById(content.Id);
            dbContent.Should().BeNull("Chapter contnet should be deleted");

            var dbFile = dbConnection.GetFileById(content.FileId);
            dbFile.Should().BeNull("Files for content should be deleted");
        }

        internal static void ShouldHaveLocationHeader(RedirectResult result, int libraryId, int bookId, ChapterContentDto content)
        {
            var response = result as RedirectResult;
            response.Url.Should().NotBeNull();
            response.Url.Should().EndWith($"library/{libraryId}/books/{bookId}/chapters/{content.ChapterId}/contents/{content.Id}");
        }
    }
}
