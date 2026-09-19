using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class BookShelfAssert(
        IBookShelfTestRepository bookShelfRepository,
        IFileTestRepository fileRepository,
        FakeFileStorage fileStorage)
    {
        private BookShelfView _bookshelf;
        private int _libraryId;
        public HttpResponseMessage _response;

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
            var bookShelf = bookShelfRepository.GetBookShelfById(bookShelfId);
            bookShelf.Should().BeNull();
            return this;
        }

        public BookShelfAssert ShouldNotHaveDeletedBookShelf(int bookShelfId)
        {
            var bookShelf = bookShelfRepository.GetBookShelfById(bookShelfId);
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
            var dbBookShelf = bookShelfRepository.GetBookShelfById(_bookshelf.Id);
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
            _bookshelf.BookCount.Should().Be(bookShelfRepository.GetBookCountByBookShelf(_bookshelf.Id));
            return this;
        }

        public BookShelfAssert ShouldHaveUpdatedBookShelfImage(int bookshelfId, byte[] newImage)
        {
            var imageUrl = bookShelfRepository.GetBookShelfImageUrl(bookshelfId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.Equal(newImage);
            return this;
        }

        public BookShelfAssert ShouldHaveChecksum(byte[] contents)
        {
            var file = _response.GetContent<FileView>().Result;
            file.Checksum.Should().Be(ChecksumTestHelper.Compute(contents));
            return this;
        }

        public BookShelfAssert ShouldHavePublicImage(int bookshelfId)
        {
            var image = bookShelfRepository.GetBookShelfImage(bookshelfId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
            return this;
        }

        public BookShelfAssert ShouldNotHaveUpdatedBookShelfImage(int bookshelfId, byte[] newImage)
        {
            var imageUrl = bookShelfRepository.GetBookShelfImageUrl(bookshelfId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotEqual(newImage);
            return this;
        }

        public BookShelfAssert ShouldHaveAddedBookShelfImage(int bookshelfId)
        {
            var imageUrl = bookShelfRepository.GetBookShelfImageUrl(bookshelfId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty();
            return this;
        }

        public BookShelfAssert ShouldHaveDeletedBookShelfImage(int bookshelfId, long imageId, string filePath)
        {
            var image = bookShelfRepository.GetBookShelfImage(bookshelfId);
            image.Should().BeNull();
            fileRepository.GetFileById(imageId).Should().BeNull();
            var file = fileStorage.DoesFileExists(filePath).Should().BeFalse();
            return this;

        }
    }
}
