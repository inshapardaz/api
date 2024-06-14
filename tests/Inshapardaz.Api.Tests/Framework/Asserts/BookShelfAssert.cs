using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters.Repositories;
using System.Data;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    internal class BookShelfAssert
    {
        private BookShelfView _bookshelf;
        private int _libraryId;

        public HttpResponseMessage _response;

        private BookShelfAssert(HttpResponseMessage response)
        {
            _response = response;
            _bookshelf = response.GetContent<BookShelfView>().Result;
        }

        private BookShelfAssert(BookShelfView view)
        {
            _bookshelf = view;
        }

        public static BookShelfAssert WithResponse(HttpResponseMessage response)
        {
            return new BookShelfAssert(response);
        }

        public BookShelfAssert InLibrary(int libraryId)
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

        internal BookShelfAssert ShouldHaveCorrectImageLocationHeader(int bookshelfId)
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

        internal BookShelfAssert ShouldHavePublicImageLink()
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

        internal BookShelfAssert WithBookCount(int count)
        {
            _bookshelf.BookCount.Should().Be(count);
            return this;
        }

        internal BookShelfAssert ShouldBeSameAs(BookShelfDto expected)
        {
            _bookshelf.Should().NotBeNull();
            _bookshelf.Name.Should().Be(expected.Name);
            return this;
        }

        internal BookShelfAssert WithReadOnlyLinks()
        {
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            ShouldNotHaveImageUpdateLink();

            return this;
        }

        internal BookShelfAssert WithEditableLinks()
        {
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            ShouldHaveImageUpdateLink();

            return this;
        }

        internal BookShelfAssert WithDeleteOnlyEditableLinks()
        {
            ShouldNotHaveUpdateLink();
            ShouldHaveDeleteLink();
            ShouldNotHaveImageUpdateLink();

            return this;
        }

        internal static BookShelfAssert FromObject(BookShelfView bookShelfView)
        {
            return new BookShelfAssert(bookShelfView);
        }

        internal static void ShouldHaveDeletedBookShelf(int bookShelfId, IDbConnection dbConnection)
        {
            var bookShelf = dbConnection.GetBookShelfById(bookShelfId);
            bookShelf.Should().BeNull();
        }

        internal static void ShouldNotHaveDeletedBookShelf(int bookShelfId, IDbConnection dbConnection)
        {
            var bookShelf = dbConnection.GetBookShelfById(bookShelfId);
            bookShelf.Should().NotBeNull();
        }

        public BookShelfAssert ShouldNotHaveDeleteLink()
        {
            _bookshelf.DeleteLink().Should().BeNull();

            return this;
        }

        internal BookShelfAssert ShouldHaveCorrectLocationHeader()
        {
            _response.Headers.Location.Should().NotBeNull();
            _response.Headers.Location.AbsolutePath.Should().EndWith($"libraries/{_libraryId}/bookshelves/{_bookshelf.Id}");
            return this;
        }

        public BookShelfAssert ShouldHaveSavedBookShelf(IDbConnection dbConnection, int accountId)
        {
            var dbBookShelf = dbConnection.GetBookShelfById(_bookshelf.Id);
            dbBookShelf.Should().NotBeNull();
            _bookshelf.Name.Should().Be(dbBookShelf.Name);
            accountId.Should().Be(dbBookShelf.AccountId);
            return this;
        }

        public BookShelfAssert ShouldHaveCorrectBookShelfRetunred(BookShelfDto bookShelf, IDbConnection dbConnection)
        {
            _bookshelf.Should().NotBeNull();
            _bookshelf.Id.Should().Be(bookShelf.Id);
            _bookshelf.Name.Should().Be(bookShelf.Name);
            _bookshelf.Description.Should().Be(bookShelf.Description);
            _bookshelf.BookCount.Should().Be(dbConnection.GetBookCountByBookShelf(_bookshelf.Id));
            return this;
        }

        internal static void ShouldHaveUpdatedBookShelfImage(int bookshelfId, byte[] newImage, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetBookShelfImageUrl(bookshelfId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.Equal(newImage);
        }

        internal static void ShouldHavePublicImage(int bookshelfId, IDbConnection dbConnection)
        {
            var image = dbConnection.GetBookShelfImage(bookshelfId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
        }

        internal static void ShouldNotHaveUpdatedBookShelfImage(int bookshelfId, byte[] newImage, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetBookShelfImageUrl(bookshelfId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotEqual(newImage);
        }

        internal static void ShouldHaveAddedBookShelfImage(int bookshelfId, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetBookShelfImageUrl(bookshelfId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty();
        }

        internal static void ShouldHaveDeletedBookShelfImage(int bookshelfId, IDbConnection dbConnection)
        {
            var image = dbConnection.GetBookShelfImage(bookshelfId);
            image.Should().BeNull();
        }
    }

    internal static class BookShelfAssertionExtensions
    {
        internal static BookShelfAssert ShouldMatch(this BookShelfView view, BookShelfDto dto)
        {
            return BookShelfAssert.FromObject(view)
                               .ShouldBeSameAs(dto);
        }
    }
}
