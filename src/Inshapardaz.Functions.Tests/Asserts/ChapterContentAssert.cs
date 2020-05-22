using FluentAssertions;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace Inshapardaz.Functions.Tests.Asserts
{
    internal class ChapterContentAssert
    {
        private ObjectResult _response;
        private readonly int _libraryId;
        private ChapterContentView _chapterContent;
        private LibraryDto _library;

        public ChapterContentAssert(CreatedResult response, int libraryId)
        {
            _response = response;
            _libraryId = libraryId;
            _chapterContent = response.Value as ChapterContentView;
        }

        public ChapterContentAssert(CreatedResult response, LibraryDto library)
        {
            _response = response;
            _libraryId = library.Id;
            _library = library;
            _chapterContent = response.Value as ChapterContentView;
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
            var response = _response as CreatedResult;
            response.Location.Should().NotBeNull();
            response.Location.Should().EndWith($"library/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterId}/contents/{_chapterContent.Id}");
            return this;
        }

        internal ChapterContentAssert ShouldHaveCorrectContentSaved(byte[] expected, IFileStorage fileStorage, IDbConnection dbConnection)
        {
            var file = dbConnection.GetFileByChapter(_chapterContent.ChapterId, _chapterContent.Language, _chapterContent.MimeType);
            var content = fileStorage.GetFile(file.FilePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.NotEqual(expected);
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

        internal ChapterContentAssert ShouldMatch(DefaultHttpRequest request, ChapterDto chapterDto)
        {
            _chapterContent.ChapterId.Should().Be(chapterDto.Id);
            _chapterContent.BookId.Should().Be(chapterDto.BookId);
            _chapterContent.MimeType.Should().Be(request.MimeType());
            _chapterContent.Language.Should().Be(request.Language());
            return this;
        }

        internal static void ShouldHaveDeletedContent(IDbConnection dbConnection, ChapterContentDto content)
        {
            var dbContent = dbConnection.GetChapterContentById(content.Id);
            dbContent.Should().BeNull("Chapter contnet should be deleted");

            var dbFile = dbConnection.GetFileById(content.FileId);
            dbFile.Should().BeNull("Files for content should be deleted");
        }
    }
}
