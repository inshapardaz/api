﻿using FluentAssertions;
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
                  .EndingWith($"library/{_libraryId}/authors/{_author.Id}/books");

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

        public AuthorAssert ShouldHaveDeleteLink()
        {
            _author.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"library/{_libraryId}/authors/{_author.Id}");
            return this;
        }

        internal static void ShouldHaveUpdatedAuthorImage(int authorId, byte[] oldImage, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetAuthorImageUrl(authorId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.NotEqual(oldImage);
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
        public static void AuthorShouldNotExist(this IDbConnection connection, int authorId)
        {
            var cat = connection.GetAuthorById(authorId);
            cat.Should().BeNull();
        }
    }
}
