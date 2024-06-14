using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    internal class BookContentAssert
    {
        private HttpResponseMessage _response;
        private readonly int _libraryId;
        private BookContentView _bookContent;
        private LibraryDto _library;

        public BookContentAssert(HttpResponseMessage response, int libraryId)
        {
            _response = response;
            _libraryId = libraryId;
            _bookContent = response.GetContent<BookContentView>().Result;
        }

        public BookContentAssert(HttpResponseMessage response, LibraryDto library)
        {
            _response = response;
            _libraryId = library.Id;
            _library = library;
            _bookContent = response.GetContent<BookContentView>().Result;
        }

        internal BookContentAssert ShouldHaveSelfLink()
        {
            _bookContent.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books/{_bookContent.BookId}/contents/{_bookContent.Id}");

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

        internal void ShouldHaveBookContent(byte[] expected, string fileName, IDbConnection db, FakeFileStorage fileStore)
        {
            var content = db.GetBookContent(_bookContent.BookId, _bookContent.Id);
            content.Should().NotBeNull();

            var file = db.GetFileById(content.FileId);
            file.FileName.Should().Be(fileName);
            fileStore.DoesFileExists(file.FilePath).Should().BeTrue();
            var fileContent = fileStore.GetFile(file.FilePath, CancellationToken.None).Result;
            fileContent.Should().BeEquivalentTo(expected);
        }

        internal BookContentAssert ShouldHaveUpdateLink()
        {
            _bookContent.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"libraries/{_libraryId}/books/{_bookContent.BookId}/contents/{_bookContent.Id}");

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

        internal BookContentAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"libraries/{_libraryId}/books/{_bookContent.BookId}/contents/{_bookContent.Id}");
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
                           .ShouldBeGet();

            return this;
        }

        internal BookContentAssert ShouldHavePublicDownloadLink()
        {
            _bookContent.Link("download")
                           .ShouldBeGet();

            return this;
        }

        internal BookContentAssert ShouldHaveSavedBookContent(IDbConnection dbConnection)
        {
            var dbContent = dbConnection.GetBookContent(_bookContent.BookId, _bookContent.Language, _bookContent.MimeType);
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
                 .EndingWith($"libraries/{_libraryId}/books/{_bookContent.BookId}/contents/{_bookContent.Id}");

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
                .EndingWith($"libraries/{_libraryId}/books/{_bookContent.BookId}");

            return this;
        }

        //internal BookContentAssert ShouldMatch( request, BookDto bookDto)
        //{
        //    _bookContent.BookId.Should().Be(bookDto.Id);
        //    _bookContent.MimeType.Should().Be(request.MimeType());
        //    _bookContent.Language.Should().Be(request.Language());
        //    return this;
        //}

        internal BookContentAssert ShouldMatch(BookContentDto content, int bookId, IDbConnection dbConnection)
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
            response.Url.Should().EndWith($"libraries/{libraryId}/books/{bookId}/files");
        }
    }
}
