using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class BookContentAssert
    {
        private HttpResponseMessage _response;
        private int _libraryId;
        private BookContentView _bookContent;
        private LibraryDto _library;
        private readonly IBookTestRepository _bookRepository;
        private readonly IFileTestRepository _fileRepository;
        private readonly FakeFileStorage _fileStorage;

        public BookContentAssert(IBookTestRepository bookRepository,
            IFileTestRepository fileRepository,
            FakeFileStorage fileStorage)
        {
            _bookRepository = bookRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public BookContentAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _bookContent = response.GetContent<BookContentView>().Result;
            return this;
        }

        public BookContentAssert ForLibrary(LibraryDto library)
        {
            _library = library;
            _libraryId = library.Id;
            return this;
        }

        public BookContentAssert ShouldHaveSelfLink()
        {
            _bookContent.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books/{_bookContent.BookId}/contents/{_bookContent.Id}");

            return this;
        }

        public BookContentAssert WithReadOnlyLinks()
        {
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            return this;
        }

        public BookContentAssert WithWriteableLinks()
        {
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            return this;
        }

        public BookContentAssert ShouldNotHaveBookContent(int bookId, string language, string mimeType)
        {
            var content = _bookRepository.GetBookContent(bookId, language, mimeType);
            content.Should().BeNull();
            return this;
        }

        public BookContentAssert ShouldHaveBookContent(int bookId, string language, string mimeType)
        {
            var content = _bookRepository.GetBookContent(bookId, language, mimeType);
            content.Should().NotBeNull();
            return this;
        }

        public BookContentAssert ShouldHaveBookContent(byte[] expected, string fileName)
        {
            var content = _bookRepository.GetBookContent(_bookContent.BookId, _bookContent.Id);
            content.Should().NotBeNull();

            var file = _fileRepository.GetFileById(content.FileId);
            file.FileName.Should().Be(fileName);
            _fileStorage.DoesFileExists(file.FilePath).Should().BeTrue();
            var fileContent = _fileStorage.GetFile(file.FilePath, CancellationToken.None).Result;
            fileContent.Should().BeEquivalentTo(expected);
            return this;
        }

        public BookContentAssert ShouldHaveUpdateLink()
        {
            _bookContent.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"libraries/{_libraryId}/books/{_bookContent.BookId}/contents/{_bookContent.Id}");

            return this;
        }

        public BookContentAssert ShouldHaveCorrectLanguage(string locale)
        {
            _bookContent.Language.Should().Be(locale);
            return this;
        }

        public BookContentAssert ShouldHaveCorrectMimeType(string mimeType)
        {
            _bookContent.MimeType.Should().Be(mimeType);
            return this;
        }

        public BookContentAssert ShouldNotHaveUpdateLink()
        {
            _bookContent.UpdateLink().Should().BeNull();
            return this;
        }

        public BookContentAssert ShouldHaveDefaultLibraryLanguage()
        {
            _bookContent.Language.Should().Be(_library.Language);
            return this;
        }

        public BookContentAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"libraries/{_libraryId}/books/{_bookContent.BookId}/contents/{_bookContent.Id}");
            return this;
        }

        public BookContentAssert ShouldHaveCorrectContents(byte[] expected)
        {
            var filePath = _bookRepository.GetBookContentPath(_bookContent.BookId, _bookContent.Language, _bookContent.MimeType);
            var content = _fileStorage.GetFile(filePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.NotEqual(expected);
            return this;
        }

        public BookContentAssert ShouldHaveCorrectContentsForMimeType(byte[] expected, string mimeType)
        {
            var filePath = _bookRepository.GetBookContentPath(_bookContent.BookId, _bookContent.Language, mimeType);
            var content = _fileStorage.GetFile(filePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.NotEqual(expected);
            return this;
        }

        public BookContentAssert ShouldHaveCorrectContentsForLanguage(byte[] expected, string language)
        {
            var filePath = _bookRepository.GetBookContentPath(_bookContent.BookId, language, _bookContent.MimeType);
            var content = _fileStorage.GetFile(filePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.NotEqual(expected);
            return this;
        }

        public BookContentAssert ShouldHavePrivateDownloadLink()
        {
            _bookContent.Link("download")
                           .ShouldBeGet();

            return this;
        }

        public BookContentAssert ShouldHavePublicDownloadLink()
        {
            _bookContent.Link("download")
                           .ShouldBeGet();

            return this;
        }

        public BookContentAssert ShouldHaveSavedBookContent()
        {
            var dbContent = _bookRepository.GetBookContent(_bookContent.BookId, _bookContent.Language, _bookContent.MimeType);
            dbContent.Should().NotBeNull();
            var dbFile = _fileRepository.GetFileById(dbContent.FileId);
            _bookContent.BookId.Should().Be(dbContent.BookId);
            _bookContent.Language.Should().Be(dbContent.Language);
            _bookContent.MimeType.Should().Be(dbFile.MimeType);

            return this;
        }

        public BookContentAssert ShouldHaveDeleteLink()
        {
            _bookContent.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"libraries/{_libraryId}/books/{_bookContent.BookId}/contents/{_bookContent.Id}");

            return this;
        }

        public BookContentAssert ShouldNotHaveDeleteLink()
        {
            _bookContent.DeleteLink().Should().BeNull();
            return this;
        }

        public BookContentAssert ShouldHaveBookLink()
        {
            _bookContent.Link("book")
                .ShouldBeGet()
                .EndingWith($"libraries/{_libraryId}/books/{_bookContent.BookId}");

            return this;
        }

        public BookContentAssert ShouldMatch(BookContentDto content, int bookId)
        {
            _bookContent.BookId.Should().Be(content.BookId);
            _bookContent.BookId.Should().Be(bookId);
            _bookContent.Language.Should().Be(content.Language);

            var dbFile = _fileRepository.GetFileById(content.FileId);
            _bookContent.MimeType.Should().Be(dbFile.MimeType);

            return this;
        }

        public BookContentAssert ShouldHaveDeletedContent(BookFileDto content, string mimeType)
        {
            var dbContent = _bookRepository.GetBookContent(content.Id, content.Language, mimeType);
            dbContent.Should().BeNull("Book contnet should be deleted");

            var dbFile = _fileRepository.GetFileById(content.FileId);
            dbFile.Should().BeNull("Files for content should be deleted");
            return this;
        }

        public BookContentAssert ShouldHaveLocationHeader(RedirectResult result, int libraryId, int bookId, BookFileDto content)
        {
            var response = result as RedirectResult;
            response.Url.Should().NotBeNull();
            response.Url.Should().EndWith($"libraries/{libraryId}/books/{bookId}/files");
            return this;
        }
    }
}
