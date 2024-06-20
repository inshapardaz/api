using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class BookShelfAssert
    {
        private BookShelfView _bookshelf;
        private int _libraryId;
        public HttpResponseMessage _response;

        private readonly IBookShelfTestRepository _bookShelfRepository;
        private readonly IFileTestRepository _fileRepository;
        private readonly FakeFileStorage _fileStorage;

        public BookShelfAssert(IBookShelfTestRepository bookShelfRepository,
            IFileTestRepository fileRepository,
            FakeFileStorage fileStorage)
        {
            _bookShelfRepository = bookShelfRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public BookShelfAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _bookshelf = response.GetContent<BookShelfView>().Result;
            return this;
        }

        public BookShelfAssert ForView(BookShelfView view)
        {
             _bookshelf = view;
            return this;
        }

        public BookShelfAssert ForLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public BookShelfAssert ShouldHaveSelfLink()
        {
            _bookshelf.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/bookshelves/{_bookshelf.Id}");

            return this;
        }

        public BookShelfAssert ShouldHaveBooksLink()
        {
            _bookshelf.Link("books")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books")
                  .ShouldHaveQueryParameter("bookshelfId", _bookshelf.Id);

            return this;
        }

        public BookShelfAssert ShouldHaveUpdateLink()
        {
            _bookshelf.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/bookshelves/{_bookshelf.Id}");

            return this;
        }

        public BookShelfAssert ShouldNotHaveUpdateLink()
        {
            _bookshelf.UpdateLink().Should().BeNull();
            return this;
        }

        public BookShelfAssert ShouldHaveDeleteLink()
        {
            _bookshelf.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/bookshelves/{_bookshelf.Id}");
            return this;
        }

        public BookShelfAssert ShouldHaveCorrectImageLocationHeader(int bookshelfId)
        {
            _response.Headers.Location.Should().NotBeNull();
            _response.Headers.Location.AbsolutePath.Should().Contain($"/files/");
            return this;
        }

        public BookShelfAssert ShouldNotHaveImageUpdateLink()
        {
            _bookshelf.Link("image-upload").Should().BeNull();
            return this;
        }

        public BookShelfAssert ShouldNotHaveImageLink()
        {
            _bookshelf.Link("image").Should().BeNull();
            return this;
        }

        public BookShelfAssert ShouldHaveImageUpdateLink()
        {
            _bookshelf.Link("image-upload")
                   .ShouldBePut()
                   .EndingWith($"libraries/{_libraryId}/bookshelves/{_bookshelf.Id}/image");
            return this;
        }

        public BookShelfAssert ShouldHavePublicImageLink()
        {
            _bookshelf.Link("image")
                .ShouldBeGet();
            return this;
        }

        public BookShelfAssert ShouldHaveImageUploadLink()
        {
            _bookshelf.Link("image-upload")
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/bookshelves/{_bookshelf.Id}/image");

            return this;
        }

        public BookShelfAssert ShouldNotHaveImageUploadLink()
        {
            _bookshelf.Link("image-upload").Should().BeNull();

            return this;
        }

        public BookShelfAssert WithBookCount(int count)
        {
            _bookshelf.BookCount.Should().Be(count);
            return this;
        }

        public BookShelfAssert ShouldBeSameAs(BookShelfDto expected)
        {
            _bookshelf.Should().NotBeNull();
            _bookshelf.Name.Should().Be(expected.Name);
            return this;
        }

        public BookShelfAssert WithReadOnlyLinks()
        {
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            ShouldNotHaveImageUpdateLink();

            return this;
        }

        public BookShelfAssert WithEditableLinks()
        {
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            ShouldHaveImageUpdateLink();

            return this;
        }

        public BookShelfAssert WithDeleteOnlyEditableLinks()
        {
            ShouldNotHaveUpdateLink();
            ShouldHaveDeleteLink();
            ShouldNotHaveImageUpdateLink();

            return this;
        }


        public BookShelfAssert ShouldHaveDeletedBookShelf(int bookShelfId)
        {
            var bookShelf = _bookShelfRepository.GetBookShelfById(bookShelfId);
            bookShelf.Should().BeNull();
            return this;
        }

        public BookShelfAssert ShouldNotHaveDeletedBookShelf(int bookShelfId)
        {
            var bookShelf = _bookShelfRepository.GetBookShelfById(bookShelfId);
            bookShelf.Should().NotBeNull();
            return this;
        }

        public BookShelfAssert ShouldNotHaveDeleteLink()
        {
            _bookshelf.DeleteLink().Should().BeNull();

            return this;
        }

        public BookShelfAssert ShouldHaveCorrectLocationHeader()
        {
            _response.Headers.Location.Should().NotBeNull();
            _response.Headers.Location.AbsolutePath.Should().EndWith($"libraries/{_libraryId}/bookshelves/{_bookshelf.Id}");
            return this;
        }

        public BookShelfAssert ShouldHaveSavedBookShelf(int accountId)
        {
            var dbBookShelf = _bookShelfRepository.GetBookShelfById(_bookshelf.Id);
            dbBookShelf.Should().NotBeNull();
            _bookshelf.Name.Should().Be(dbBookShelf.Name);
            accountId.Should().Be(dbBookShelf.AccountId);
            return this;
        }

        public BookShelfAssert ShouldHaveCorrectBookShelfRetunred(BookShelfDto bookShelf)
        {
            _bookshelf.Should().NotBeNull();
            _bookshelf.Id.Should().Be(bookShelf.Id);
            _bookshelf.Name.Should().Be(bookShelf.Name);
            _bookshelf.Description.Should().Be(bookShelf.Description);
            _bookshelf.BookCount.Should().Be(_bookShelfRepository.GetBookCountByBookShelf(_bookshelf.Id));
            return this;
        }

        public BookShelfAssert ShouldHaveUpdatedBookShelfImage(int bookshelfId, byte[] newImage)
        {
            var imageUrl = _bookShelfRepository.GetBookShelfImageUrl(bookshelfId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.Equal(newImage);
            return this;
        }

        public BookShelfAssert ShouldHavePublicImage(int bookshelfId)
        {
            var image = _bookShelfRepository.GetBookShelfImage(bookshelfId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
            return this;
        }

        public BookShelfAssert ShouldNotHaveUpdatedBookShelfImage(int bookshelfId, byte[] newImage)
        {
            var imageUrl = _bookShelfRepository.GetBookShelfImageUrl(bookshelfId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotEqual(newImage);
            return this;
        }

        public BookShelfAssert ShouldHaveAddedBookShelfImage(int bookshelfId)
        {
            var imageUrl = _bookShelfRepository.GetBookShelfImageUrl(bookshelfId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty();
            return this;
        }

        public BookShelfAssert ShouldHaveDeletedBookShelfImage(int bookshelfId, long imageId, string filePath)
        {
            var image = _bookShelfRepository.GetBookShelfImage(bookshelfId);
            image.Should().BeNull();
            _fileRepository.GetFileById(imageId).Should().BeNull();
            var file = _fileStorage.DoesFileExists(filePath).Should().BeFalse();
            return this;

        }
    }
}
