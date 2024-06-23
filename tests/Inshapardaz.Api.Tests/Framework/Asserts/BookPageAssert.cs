using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using System;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class BookPageAssert
    {
        private readonly IBookPageTestRepository _bookPageRepository;
        private readonly IFileTestRepository _fileRepository;
        private readonly FakeFileStorage _fileStorage;
        public HttpResponseMessage _response;
        private BookPageView _bookPage;
        private int _libraryId;

        public BookPageAssert(IBookPageTestRepository bookPageRepository,
            IFileTestRepository fileRepository,
            FakeFileStorage fileStorage)
        {
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
            _bookPageRepository = bookPageRepository;
        }


        public BookPageAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _bookPage = response.GetContent<BookPageView>().Result;
            return this;
        }

        public BookPageAssert ForView(BookPageView view)
        {
            _bookPage = view;
            return this;
        }

        public BookPageAssert ForLibrary(int libraryId)
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

        public BookPageAssert ShouldHaveNoBookPage(int bookId, long pageId, long? imageId)
        {
            var page = _bookPageRepository.GetBookPageById(bookId, pageId);
            page.Should().BeNull();

            if (imageId != null)
            {
                var image = _fileRepository.GetFileById(imageId.Value);
                image.Should().BeNull();
            }
            return this;
        }

        public BookPageAssert ShouldHaveAssignedRecently()
        {
            _bookPage.WriterAssignTimeStamp.Should().NotBeNull();
            _bookPage.WriterAssignTimeStamp.Value.Should().BeWithin(TimeSpan.FromMinutes(1));
            return this;
        }

        public BookPageAssert BookPageShouldExist(int bookId, int pageNumber)
        {
            var page = _bookPageRepository.GetBookPageByNumber(bookId, pageNumber);
            page.Should().NotBeNull();

            if (page.ImageId != null)
            {
                var image = _fileRepository.GetFileById(page.ImageId.Value);
                image.Should().NotBeNull();
            }
            return this;
        }

        public BookPageAssert ShouldHaveNoBookPageImage(int bookId, int pageNumber, long imageId)
        {
            var page = _bookPageRepository.GetBookPageByNumber(bookId, pageNumber);
            page.ImageId.Should().BeNull();

            var image = _fileRepository.GetFileById(imageId);
            image.Should().BeNull();
            return this;
        }

        public BookPageAssert ShouldNotHaveCorrectLocationHeader()
        {
            _response.Headers.Location.Should().BeNull();
            return this;
        }

        public BookPageAssert ShouldHaveSavedPage()
        {
            _bookPageRepository.GetBookPageByNumber(_bookPage.BookId, _bookPage.SequenceNumber);
            return this;
        }

        public BookPageAssert ShouldHaveUpdatedBookPageImage(int bookId, int pageNumber, byte[] newImage)
        {
            var page = _bookPageRepository.GetBookPageByNumber(bookId, pageNumber);
            page.ImageId.Should().BeGreaterThan(0);

            var image = _fileRepository.GetFileById(page.ImageId.Value);
            image.Should().NotBeNull();

            var content = _fileStorage.GetFile(image.FilePath, CancellationToken.None).Result;
            content.Should().BeEquivalentTo(newImage);
            return this;
        }

        public BookPageAssert ShouldHaveCorrectImageLocationHeader(HttpResponseMessage response, long imageId)
        {
            string location = response.Headers.Location.AbsoluteUri;
            location.Should().NotBeEmpty();
            location.Should().EndWith($"/files/{imageId}");
            return this;
        }

        public BookPageAssert ShouldHaveAddedBookPageImage(int bookId, int pageNumber)
        {
            var page = _bookPageRepository.GetBookPageByNumber(bookId, pageNumber);
            page.ImageId.Should().BeGreaterThan(0);

            var image = _fileRepository.GetFileById(page.ImageId.Value);
            image.Should().NotBeNull();
            return this;
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

        public BookPageAssert ShouldHaveImageLink(long imageId)
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

        public BookPageAssert ShouldMatch(BookPageView view, int pageNumber = -1)
        {
            _bookPage.Text.Should().Be(view.Text);
            _bookPage.BookId.Should().Be(view.BookId);
            _bookPage.SequenceNumber.Should().Be(pageNumber >= 0 ? pageNumber : view.SequenceNumber);
            return this;
        }

        public BookPageAssert ShouldMatch(BookPageDto dto)
        {
            _bookPage.Text.Should().Be(dto.Text);
            _bookPage.BookId.Should().Be(dto.BookId);
            _bookPage.SequenceNumber.Should().Be(dto.SequenceNumber);
            return this;
        }

        public BookPageAssert ShouldHaveBookPageContent(string text)
        {
            var page = _bookPageRepository.GetBookPageByNumber(_bookPage.BookId, _bookPage.SequenceNumber);
            page.ContentId.Should().NotBeNull();

            var file = _fileRepository.GetFileById(page.ContentId.Value);
            var fileContents = _fileStorage.GetTextFile(file.FilePath, CancellationToken.None).Result;
            fileContents.Should().BeEquivalentTo(text);
            return this;
        }

        public BookPageAssert ShouldHaveNoBookPageContent(long contentId, string filePath)
        {
            var file = _fileRepository.GetFileById(contentId);
            file.Should().BeNull();
            _fileStorage.DoesFileExists(filePath).Should().BeFalse();
            return this;
        }
    }
}
