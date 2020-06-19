using FluentAssertions;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading;

namespace Inshapardaz.Functions.Tests.Asserts
{
    public class AuthorAssert
    {
        private AuthorView _author;
        private int _libraryId;

        public ObjectResult _response;

        private AuthorAssert(ObjectResult response)
        {
            _response = response;
            _author = response.Value as AuthorView;
        }

        private AuthorAssert(AuthorView view)
        {
            _author = view;
        }

        public static AuthorAssert WithResponse(ObjectResult response)
        {
            return new AuthorAssert(response);
        }

        public AuthorAssert InLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public AuthorAssert ShouldHaveSelfLink()
        {
            _author.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"library/{_libraryId}/authors/{_author.Id}");

            return this;
        }

        public AuthorAssert ShouldHaveBooksLink()
        {
            _author.Link("books")
                  .ShouldBeGet()
                  .EndingWith($"library/{_libraryId}/books")
                  .ShouldHaveQueryParameter("authorId", _author.Id);

            return this;
        }

        public AuthorAssert ShouldHaveUpdateLink()
        {
            _author.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"library/{_libraryId}/authors/{_author.Id}");

            return this;
        }

        public AuthorAssert ShouldNotHaveUpdateLink()
        {
            _author.UpdateLink().Should().BeNull();
            return this;
        }

        internal AuthorAssert ShouldHaveCorrectImageLocationHeader(int authorId)
        {
            var response = _response as CreatedResult;
            response.Location.Should().NotBeNull();
            return this;
        }

        public AuthorAssert ShouldNotHaveImageUpdateLink()
        {
            _author.Link("image-upload").Should().BeNull();
            return this;
        }

        public AuthorAssert ShouldHaveImageUpdateLink()
        {
            _author.Link("image-upload")
                   .ShouldBePut()
                   .EndingWith($"library/{_libraryId}/authors/{_author.Id}/image");
            return this;
        }

        internal void ShouldHavePublicImageLink()
        {
            _author.Link("image")
                .ShouldBeGet()
                .Href.Should()
                .StartWith(ConfigurationSettings.CDNAddress);
        }

        public AuthorAssert ShouldHaveDeleteLink()
        {
            _author.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"library/{_libraryId}/authors/{_author.Id}");
            return this;
        }

        internal AuthorAssert WithBookCount(int count)
        {
            _author.BookCount.Should().Be(count);
            return this;
        }

        internal AuthorAssert ShouldBeSameAs(AuthorDto expected)
        {
            _author.Should().NotBeNull();
            _author.Name.Should().Be(expected.Name);
            return this;
        }

        internal AuthorAssert WithReadOnlyLinks()
        {
            ShouldHaveSelfLink();
            ShouldHaveBooksLink();
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            ShouldNotHaveImageUpdateLink();

            return this;
        }

        internal AuthorAssert WithEditableLinks()
        {
            ShouldHaveSelfLink();
            ShouldHaveBooksLink();
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            ShouldHaveImageUpdateLink();

            return this;
        }

        internal static AuthorAssert FromObject(AuthorView author)
        {
            return new AuthorAssert(author);
        }

        internal static void ShouldHaveUpdatedAuthorImage(int authorId, byte[] oldImage, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetAuthorImageUrl(authorId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.NotEqual(oldImage);
        }

        internal static void ShouldHavePublicImage(int authorId, IDbConnection dbConnection)
        {
            var image = dbConnection.GetAuthorImage(authorId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
        }

        internal static void ShouldNotHaveUpdatedAuthorImage(int authorId, byte[] oldImage, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetAuthorImageUrl(authorId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().Equal(oldImage);
        }

        internal static void ShouldHaveAddedAuthorImage(int authorId, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetAuthorImageUrl(authorId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty();
        }

        internal static void ShouldHaveDeletedAuthorImage(int authorId, IDbConnection dbConnection)
        {
            var image = dbConnection.GetAuthorImage(authorId);
            image.Should().BeNull();
        }

        internal static void ShouldHaveDeletedAuthor(int authorId, IDbConnection dbConnection)
        {
            var author = dbConnection.GetAuthorById(authorId);
            author.Should().BeNull();
        }

        internal static void ShouldNotHaveDeletedAuthor(int authorId, IDbConnection dbConnection)
        {
            var author = dbConnection.GetAuthorById(authorId);
            author.Should().NotBeNull();
        }

        public AuthorAssert ShouldNotHaveDeleteLink()
        {
            _author.DeleteLink().Should().BeNull();

            return this;
        }

        public AuthorAssert ShouldHaveImageUploadLink()
        {
            _author.Link("image-upload")
                  .ShouldBePut()
                  .EndingWith($"library/{_libraryId}/authors/{_author.Id}/image");

            return this;
        }

        public AuthorAssert ShouldNotHaveImageUploadLink()
        {
            _author.Link("image-upload").Should().BeNull();

            return this;
        }

        internal AuthorAssert ShouldHaveCorrectLocationHeader()
        {
            var response = _response as CreatedResult;
            response.Location.Should().NotBeNull();
            response.Location.Should().EndWith($"library/{_libraryId}/authors/{_author.Id}");
            return this;
        }

        public AuthorAssert ShouldHaveSavedAuthor(IDbConnection dbConnection)
        {
            var dbAuthor = dbConnection.GetAuthorById(_author.Id);
            dbAuthor.Should().NotBeNull();
            _author.Name.Should().Be(dbAuthor.Name);
            return this;
        }

        public AuthorAssert ShouldHaveCorrectAuthorRetunred(AuthorDto author, IDbConnection dbConnection)
        {
            _author.Should().NotBeNull();
            _author.Id.Should().Be(author.Id);
            _author.Name.Should().Be(author.Name);
            _author.BookCount.Should().Be(dbConnection.GetBookCountByAuthor(_author.Id));
            return this;
        }
    }

    public static class AuthorAssertionExtensions
    {
        public static AuthorAssert ShouldMatch(this AuthorView view, AuthorDto dto)
        {
            return AuthorAssert.FromObject(view)
                               .ShouldBeSameAs(dto);
        }
    }
}
