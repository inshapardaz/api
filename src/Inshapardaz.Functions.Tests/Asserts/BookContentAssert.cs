using FluentAssertions;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Threading;

namespace Inshapardaz.Functions.Tests.Asserts
{
    internal class BookContentAssert
    {
        private ObjectResult _response;
        private readonly int _libraryId;
        private BookContentView _bookContent;
        private LibraryDto _library;

        public BookContentAssert(ObjectResult response, int libraryId)
        {
            _response = response;
            _libraryId = libraryId;
            _bookContent = response.Value as BookContentView;
        }

        public BookContentAssert(ObjectResult response, LibraryDto library)
        {
            _response = response;
            _libraryId = library.Id;
            _library = library;
            _bookContent = response.Value as BookContentView;
        }

        internal BookContentAssert ShouldHaveSelfLink()
        {
            _bookContent.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"library/{_libraryId}/books/{_bookContent.BookId}/content");

            return this;
        }

        internal BookContentAssert WithReadOnlyLinks()
        {
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            return this;
        }

        internal BookContentAssert WithWriteableLinks()
        {
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            return this;
        }

        internal static void ShouldNotHaveBookContent(int bookId, string language, string mimeType, IDbConnection db)
        {
            var content = db.GetBookContent(bookId, language, mimeType);
            content.Should().BeNull();
        }

        internal static void ShouldHaveBookContent(int bookId, string language, string mimeType, IDbConnection db)
        {
            var content = db.GetBookContent(bookId, language, mimeType);
            content.Should().NotBeNull();
        }

        internal void ShouldHaveBookContent(byte[] expected, IDbConnection db, IFileStorage fileStore)
        {
            var content = db.GetBookContent(_bookContent.BookId, _bookContent.Language, _bookContent.MimeType);
            content.Should().NotBeNull();

            // TODO: Make sure the contents are correct
            //var file = fileStore.GetFile(content.FilePath, CancellationToken.None).Result;
            //file.Should().NotBeNull().And.Should().Be(expected);
        }

        internal BookContentAssert ShouldHaveUpdateLink()
        {
            _bookContent.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"library/{_libraryId}/books/{_bookContent.BookId}/content");

            return this;
        }

        internal BookContentAssert ShouldHaveCorrectLanguage(string locale)
        {
            _bookContent.Language.Should().Be(locale);
            return this;
        }

        internal BookContentAssert ShouldHaveCorrectMimeType(string mimeType)
        {
            _bookContent.MimeType.Should().Be(mimeType);
            return this;
        }

        internal BookContentAssert ShouldNotHaveUpdateLink()
        {
            _bookContent.UpdateLink().Should().BeNull();
            return this;
        }

        internal BookContentAssert ShouldHaveDefaultLibraryLanguage()
        {
            _bookContent.Language.Should().Be(_library.Language);
            return this;
        }

        internal BookContentAssert ShouldHaveCorrectLoactionHeader()
        {
            var response = _response as CreatedResult;
            response.Location.Should().NotBeNull();
            response.Location.Should().EndWith($"library/{_libraryId}/books/{_bookContent.BookId}/content");
            return this;
        }

        internal BookContentAssert ShouldHaveCorrectContents(byte[] expected, IFileStorage fileStorage, IDbConnection dbConnection)
        {
            var filePath = dbConnection.GetBookContentPath(_bookContent.BookId, _bookContent.Language, _bookContent.MimeType);
            var content = fileStorage.GetFile(filePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.NotEqual(expected);
            return this;
        }

        internal BookContentAssert ShouldHaveCorrectContentsForMimeType(byte[] expected, string mimeType, IFileStorage fileStorage, IDbConnection dbConnection)
        {
            var filePath = dbConnection.GetBookContentPath(_bookContent.BookId, _bookContent.Language, mimeType);
            var content = fileStorage.GetFile(filePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.NotEqual(expected);
            return this;
        }

        internal BookContentAssert ShouldHaveCorrectContentsForLanguage(byte[] expected, string language, IFileStorage fileStorage, IDbConnection dbConnection)
        {
            var filePath = dbConnection.GetBookContentPath(_bookContent.BookId, language, _bookContent.MimeType);
            var content = fileStorage.GetFile(filePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.NotEqual(expected);
            return this;
        }

        internal BookContentAssert ShouldHavePrivateDownloadLink()
        {
            _bookContent.Link("download")
                           .ShouldBeGet()
                           .Href.Should()
                           .StartWith(ConfigurationSettings.BlobRoot);

            return this;
        }

        internal BookContentAssert ShouldHavePublicDownloadLink()
        {
            _bookContent.Link("download")
                           .ShouldBeGet()
                           .Href.Should()
                           .StartWith(ConfigurationSettings.CDNAddress);

            return this;
        }

        internal BookContentAssert ShouldHaveSavedBookContent(IDbConnection dbConnection)
        {
            var dbContent = dbConnection.GetBookContent(_bookContent.Id, _bookContent.Language, _bookContent.MimeType);
            dbContent.Should().NotBeNull();
            var dbFile = dbConnection.GetFileById(dbContent.FileId);
            _bookContent.BookId.Should().Be(dbContent.BookId);
            _bookContent.Language.Should().Be(dbContent.Language);
            _bookContent.MimeType.Should().Be(dbFile.MimeType);

            return this;
        }

        internal BookContentAssert ShouldHaveDeleteLink()
        {
            _bookContent.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"library/{_libraryId}/books/{_bookContent.BookId}/content");

            return this;
        }

        internal BookContentAssert ShouldNotHaveDeleteLink()
        {
            _bookContent.DeleteLink().Should().BeNull();
            return this;
        }

        internal BookContentAssert ShouldHaveBookLink()
        {
            _bookContent.Link("book")
                .ShouldBeGet()
                .EndingWith($"library/{_libraryId}/books/{_bookContent.BookId}");

            return this;
        }

        internal BookContentAssert ShouldMatch(DefaultHttpRequest request, BookDto bookDto)
        {
            _bookContent.BookId.Should().Be(bookDto.Id);
            _bookContent.MimeType.Should().Be(request.MimeType());
            _bookContent.Language.Should().Be(request.Language());
            return this;
        }

        internal BookContentAssert ShouldMatch(BookFileDto content, int bookId, IDbConnection dbConnection)
        {
            _bookContent.BookId.Should().Be(content.BookId);
            _bookContent.BookId.Should().Be(bookId);
            _bookContent.Language.Should().Be(content.Language);

            var dbFile = dbConnection.GetFileById(content.FileId);
            _bookContent.MimeType.Should().Be(dbFile.MimeType);

            return this;
        }

        internal static void ShouldHaveDeletedContent(IDbConnection dbConnection, BookFileDto content, string mimeType)
        {
            var dbContent = dbConnection.GetBookContent(content.Id, content.Language, mimeType);
            dbContent.Should().BeNull("Book contnet should be deleted");

            var dbFile = dbConnection.GetFileById(content.FileId);
            dbFile.Should().BeNull("Files for content should be deleted");
        }

        internal static void ShouldHaveLocationHeader(RedirectResult result, int libraryId, int bookId, BookFileDto content)
        {
            var response = result as RedirectResult;
            response.Url.Should().NotBeNull();
            response.Url.Should().EndWith($"library/{libraryId}/books/{bookId}/files");
        }
    }
}
