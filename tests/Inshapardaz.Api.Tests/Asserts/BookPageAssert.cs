using DocumentFormat.OpenXml.Wordprocessing;
using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using System;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Asserts
{
    public class BookPageAssert
    {
        internal static BookPageAssert FromResponse(HttpResponseMessage response, int libraryId)
        {
            return new BookPageAssert(response) { _libraryId = libraryId };
        }

        public static BookPageAssert FromObject(BookPageView view)
        {
            return new BookPageAssert(view);
        }

        public HttpResponseMessage _response;
        private BookPageView _bookPage;
        private int _libraryId;

        public BookPageAssert(HttpResponseMessage response)
        {
            _response = response;
            _bookPage = response.GetContent<BookPageView>().Result;
        }

        public BookPageAssert(BookPageView view)
        {
            _bookPage = view;
        }

        public BookPageAssert InLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public BookPageAssert ShouldHaveCorrectLocationHeader()
        {
            string location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeEmpty();
            location.Should().EndWith($"libraries/{_libraryId}/books/{_bookPage.BookId}/pages/{_bookPage.SequenceNumber}");
            return this;
        }

        internal static void ShouldHaveNoBookPage(int bookId, long pageId, int? imageId, IDbConnection databaseConnection, FakeFileStorage fileStore)
        {
            var page = databaseConnection.GetBookPageById(bookId, pageId);
            page.Should().BeNull();

            if (imageId != null)
            {
                var image = databaseConnection.GetFileById(imageId.Value);
                image.Should().BeNull();
            }
        }

        internal BookPageAssert ShouldHaveAssignedRecently()
        {
            _bookPage.WriterAssignTimeStamp.Should().NotBeNull();
            _bookPage.WriterAssignTimeStamp.Value.Should().BeWithin(TimeSpan.FromMinutes(1));
            return this;
        }

        internal static void BookPageShouldExist(int bookId, int pageNumber, IDbConnection databaseConnection)
        {
            var page = databaseConnection.GetBookPageByNumber(bookId, pageNumber);
            page.Should().NotBeNull();

            if (page.ImageId != null)
            {
                var image = databaseConnection.GetFileById(page.ImageId.Value);
                image.Should().NotBeNull();
            }
        }

        internal static void ShouldHaveNoBookPageImage(int bookId, int pageNumber, int imageId, IDbConnection databaseConnection, FakeFileStorage fileStore)
        {
            var page = databaseConnection.GetBookPageByNumber(bookId, pageNumber);
            page.ImageId.Should().BeNull();

            var image = databaseConnection.GetFileById(imageId);
            image.Should().BeNull();
        }

        public BookPageAssert ShouldNotHaveCorrectLocationHeader()
        {
            _response.Headers.Location.Should().BeNull();
            return this;
        }

        public void ShouldHaveSavedPage(IDbConnection databaseConnection)
        {
            databaseConnection.GetBookPageByNumber(_bookPage.BookId, _bookPage.SequenceNumber);
        }

        internal static void ShouldHaveUpdatedBookPageImage(int bookId, int pageNumber, byte[] newImage, IDbConnection databaseConnection, FakeFileStorage fileStore)
        {
            var page = databaseConnection.GetBookPageByNumber(bookId, pageNumber);
            page.ImageId.Should().BeGreaterThan(0);

            var image = databaseConnection.GetFileById(page.ImageId.Value);
            image.Should().NotBeNull();

            var content = fileStore.GetFile(image.FilePath, CancellationToken.None).Result;
            content.Should().BeEquivalentTo(newImage);
        }

        internal static void ShouldHaveCorrectImageLocationHeader(HttpResponseMessage response, int imageId)
        {
            string location = response.Headers.Location.AbsoluteUri;
            location.Should().NotBeEmpty();
            location.Should().EndWith($"/files/{imageId}");
        }

        internal static void ShouldHaveAddedBookPageImage(int bookId, int pageNumber, IDbConnection databaseConnection, FakeFileStorage fileStore)
        {
            var page = databaseConnection.GetBookPageByNumber(bookId, pageNumber);
            page.ImageId.Should().BeGreaterThan(0);

            var image = databaseConnection.GetFileById(page.ImageId.Value);
            image.Should().NotBeNull();
        }

        public BookPageAssert ShouldHaveSelfLink()
        {
            _bookPage.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books/{_bookPage.BookId}/pages/{_bookPage.SequenceNumber}");
            return this;
        }

        public BookPageAssert ShouldHaveBookLink()
        {
            _bookPage.Link("book")
                .ShouldBeGet()
                .EndingWith($"libraries/{_libraryId}/books/{_bookPage.BookId}");

            return this;
        }

        public BookPageAssert ShouldHaveUpdateLink()
        {
            _bookPage.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/books/{_bookPage.BookId}/pages/{_bookPage.SequenceNumber}");
            return this;
        }

        public BookPageAssert ShouldNotHaveUpdateLink()
        {
            _bookPage.UpdateLink().Should().BeNull();
            return this;
        }

        public BookPageAssert ShouldHaveDeleteLink()
        {
            _bookPage.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/books/{_bookPage.BookId}/pages/{_bookPage.SequenceNumber}");
            return this;
        }

        public BookPageAssert ShouldNotHaveDeleteLink()
        {
            _bookPage.DeleteLink().Should().BeNull();
            return this;
        }

        public BookPageAssert ShouldHaveNoNextLink()
        {
            _bookPage.Link("next").Should().BeNull();
            return this;
        }

        public BookPageAssert ShouldHavePageNumber(int pageNumber)
        {
            _bookPage.SequenceNumber.Should().Be(pageNumber);
            return this;
        }

        public BookPageAssert ShouldHaveNextLinkForPageNumber(int pageNumber)
        {
            _bookPage.Link("next")
                .ShouldBeGet()
                .EndingWith($"/pages/{pageNumber}");
            return this;
        }

        public BookPageAssert ShouldHaveNoPreviousLink()
        {
            _bookPage.Link("previous").Should().BeNull();
            return this;
        }

        public BookPageAssert ShouldHavePreviousLinkForPageNumber(int pageNumber)
        {
            _bookPage.Link("previous")
                .ShouldBeGet()
                .EndingWith($"/pages/{pageNumber}");
            return this;
        }

        public BookPageAssert ShouldHaveImageLink(int imageId)
        {
            _bookPage.Link("image")
                  .ShouldBeGet()
                  .EndingWith($"files/{imageId}");
            return this;
        }

        public BookPageAssert ShouldNotHaveImageLink()
        {
            _bookPage.Link("image").Should().BeNull();
            return this;
        }

        public BookPageAssert ShouldHaveImageUpdateLink()
        {
            _bookPage.Link("image-upload")
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/books/{_bookPage.BookId}/pages/{_bookPage.SequenceNumber}/image");
            return this;
        }

        public BookPageAssert ShouldNotHaveImageUpdateLink()
        {
            _bookPage.Link("image-update").Should().BeNull();
            return this;
        }

        public BookPageAssert ShouldHaveImageDeleteLink()
        {
            _bookPage.Link("image-delete")
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/books/{_bookPage.BookId}/pages/{_bookPage.SequenceNumber}/image");
            return this;
        }

        public BookPageAssert ShouldNotHaveImageDeleteLink()
        {
            _bookPage.Link("image-delete").Should().BeNull();
            return this;
        }

        public void ShouldMatch(BookPageView view, int pageNumber = -1)
        {
            _bookPage.Text.Should().Be(view.Text);
            _bookPage.BookId.Should().Be(view.BookId);
            _bookPage.SequenceNumber.Should().Be(pageNumber >= 0 ? pageNumber : view.SequenceNumber);
        }

        public BookPageAssert ShouldMatch(BookPageDto dto)
        {
            _bookPage.Text.Should().Be(dto.Text);
            _bookPage.BookId.Should().Be(dto.BookId);
            _bookPage.SequenceNumber.Should().Be(dto.SequenceNumber);
            return this;
        }

        internal void ShouldHaveBookPageContent(string text, IDbConnection db, FakeFileStorage fileStore)
        {
            var page = db.GetBookPageByNumber(_bookPage.BookId, _bookPage.SequenceNumber);
            page.ContentId.Should().NotBeNull();

            var file = db.GetFileById(page.ContentId.Value);
            var fileContents = fileStore.GetTextFile(file.FilePath, CancellationToken.None).Result;
            fileContents.Should().BeEquivalentTo(text);
        }

        public static void ShouldHaveNoBookPageContent(int contentId, string filePath, IDbConnection db, FakeFileStorage fileStore)
        {
            var file = db.GetFileById(contentId);
            file.Should().BeNull();
            fileStore.DoesFileExists(filePath).Should().BeFalse();
        }
    }

    public static class BookPageAssertionExtensions
    {
        public static BookPageAssert ShouldMatch(this BookPageView view, BookPageDto dto)
        {
            return BookPageAssert.FromObject(view)
                               .ShouldMatch(dto);
        }
    }
}
