using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using System.Data;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Asserts
{
    public class AuthorAssert
    {
        private AuthorView _author;
        private int _libraryId;

        public HttpResponseMessage _response;

        private AuthorAssert(HttpResponseMessage response)
        {
            _response = response;
            _author = response.GetContent<AuthorView>().Result;
        }

        private AuthorAssert(AuthorView view)
        {
            _author = view;
        }

        public static AuthorAssert WithResponse(HttpResponseMessage response)
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
                  .EndingWith($"libraries/{_libraryId}/authors/{_author.Id}");

            return this;
        }

        public AuthorAssert ShouldHaveBooksLink()
        {
            _author.Link("books")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books")
                  .ShouldHaveQueryParameter("authorId", _author.Id);

            return this;
        }

        public AuthorAssert ShouldHaveUpdateLink()
        {
            _author.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/authors/{_author.Id}");

            return this;
        }

        public AuthorAssert ShouldNotHaveUpdateLink()
        {
            _author.UpdateLink().Should().BeNull();
            return this;
        }

        internal AuthorAssert ShouldHaveCorrectImageLocationHeader(int authorId)
        {
            _response.Headers.Location.Should().NotBeNull();
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
                   .EndingWith($"libraries/{_libraryId}/authors/{_author.Id}/image");
            return this;
        }

        internal void ShouldHavePublicImageLink()
        {
            _author.Link("image")
                .ShouldBeGet()
                .Href.Should()
                .NotBeNullOrEmpty();
            //.StartWith(Settings.CDNAddress);
        }

        internal void ShouldNotHaveImageLink()
        {
            _author.Link("image").Should().BeNull();
        }

        public AuthorAssert ShouldHaveDeleteLink()
        {
            _author.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/authors/{_author.Id}");
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

        internal AuthorAssert WithEditableLinks(Role role)
        {
            ShouldHaveSelfLink();
            ShouldHaveBooksLink();
            ShouldHaveUpdateLink();
            if (role == Role.Admin || role == Role.LibraryAdmin)
            {
                ShouldHaveDeleteLink();
            }
            else
            {
                ShouldNotHaveDeleteLink();
            }
            ShouldHaveImageUpdateLink();

            return this;
        }

        internal static AuthorAssert FromObject(AuthorView author)
        {
            return new AuthorAssert(author);
        }

        internal static void ShouldHaveUpdatedAuthorImage(int authorId, byte[] newImage, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetAuthorImageUrl(authorId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.Equal(newImage);
        }

        internal static void ShouldHavePublicImage(int authorId, IDbConnection dbConnection)
        {
            var image = dbConnection.GetAuthorImage(authorId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
        }

        internal static void ShouldNotHaveUpdatedAuthorImage(int authorId, byte[] newImage, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetAuthorImageUrl(authorId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotEqual(newImage);
        }

        internal static void ShouldHaveAddedAuthorImage(int authorId, byte[] newImage, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetAuthorImageUrl(authorId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty().And.Equal(newImage);
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
                  .EndingWith($"libraries/{_libraryId}/authors/{_author.Id}/image");

            return this;
        }

        public AuthorAssert ShouldNotHaveImageUploadLink()
        {
            _author.Link("image-upload").Should().BeNull();

            return this;
        }

        internal AuthorAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location;
            location.Should().NotBeNull();
            location.AbsoluteUri.Should().EndWith($"libraries/{_libraryId}/authors/{_author.Id}");
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
            _author.ArticleCount.Should().Be(dbConnection.GetArticleCountByAuthor(_author.Id));
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
