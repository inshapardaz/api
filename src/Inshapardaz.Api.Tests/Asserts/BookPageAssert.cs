using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data;
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

        public BookPageAssert ShouldHaveCorrectLoactionHeader()
        {
            string location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeEmpty();
            location.Should().EndWith($"library/{_libraryId}/books/{_bookPage.BookId}/pages/{_bookPage.PageNumber}");
            return this;
        }

        public BookPageAssert ShouldNotHaveCorrectLoactionHeader()
        {
            _response.Headers.Location.Should().BeNull();
            return this;
        }

        public void ShouldHaveSavedPage(IDbConnection databaseConnection)
        {
            databaseConnection.GetBookPageByNumber(_bookPage.BookId, _bookPage.PageNumber);
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

        internal static void ShouldHaveCorrectImageLocationHeader(HttpResponseMessage response, int libraryId, int bookId, int pageNumber)
        {
            string location = response.Headers.Location.AbsoluteUri;
            location.Should().NotBeEmpty();
            location.Should().EndWith($"library/{libraryId}/books/{bookId}/pages/{pageNumber}/image");
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
                  .EndingWith($"library/{_libraryId}/books/{_bookPage.BookId}/pages/{_bookPage.PageNumber}");
            return this;
        }

        public BookPageAssert ShouldHaveBookLink()
        {
            _bookPage.Link("book")
                .ShouldBeGet()
                .EndingWith($"library/{_libraryId}/books/{_bookPage.BookId}");

            return this;
        }

        public BookPageAssert ShouldHaveUpdateLink()
        {
            _bookPage.Link("update")
                  .ShouldBePut()
                  .EndingWith($"library/{_libraryId}/books/{_bookPage.BookId}/pages/{_bookPage.PageNumber}");
            return this;
        }

        public BookPageAssert ShouldHaveDeleteLink()
        {
            _bookPage.Link("delete")
                  .ShouldBeDelete()
                  .EndingWith($"library/{_libraryId}/books/{_bookPage.BookId}/pages/{_bookPage.PageNumber}");
            return this;
        }

        public BookPageAssert ShouldHaveImageLink()
        {
            _bookPage.Link("image")
                  .ShouldBeGet()
                  .EndingWith($"library/{_libraryId}/books/{_bookPage.BookId}/pages/{_bookPage.PageNumber}/image");
            return this;
        }

        public void ShouldMatch(BookPageView view)
        {
            _bookPage.Text.Should().Be(view.Text);
            _bookPage.BookId.Should().Be(view.BookId);
            _bookPage.PageNumber.Should().Be(view.PageNumber);
        }
    }
}
