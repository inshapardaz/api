using FluentAssertions;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Inshapardaz.Functions.Tests.Asserts
{
    public class BookAssert
    {
        private BookView _book;
        private int _libraryId;

        public ObjectResult _response;

        public BookAssert(ObjectResult response)
        {
            _response = response;
            _book = response.Value as BookView;
        }

        internal static BookAssert WithResponse(ObjectResult response)
        {
            return new BookAssert(response);
        }

        public BookAssert InLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public BookAssert ShouldHaveSelfLink()
        {
            _book.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"library/{_libraryId}/books/{_book.Id}");

            return this;
        }

        public BookAssert ShouldHaveAuthorLink()
        {
            _book.Link("author")
                  .ShouldBeGet()
                  .EndingWith($"library/{_libraryId}/authors/{_book.AuthorId}");

            return this;
        }

        public BookAssert ShouldHaveUpdateLink()
        {
            _book.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"library/{_libraryId}/books/{_book.Id}");

            return this;
        }

        public BookAssert ShouldNotHaveUpdateLink()
        {
            _book.UpdateLink().Should().BeNull();
            return this;
        }

        public BookAssert ShouldNotHaveImageUpdateLink()
        {
            _book.Link("image-upload").Should().BeNull();
            return this;
        }

        public BookAssert ShouldHaveImageUpdateLink()
        {
            _book.Link("image-upload")
                   .ShouldBePut()
                   .EndingWith($"library/{_libraryId}/books/{_book.Id}/image");
            return this;
        }

        public BookAssert ShouldHaveDeleteLink()
        {
            _book.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"library/{_libraryId}/books/{_book.Id}");
            return this;
        }

        internal void ShouldHavePublicImageLink()
        {
            _book.Link("image")
                .ShouldBeGet()
                .Href.Should()
                .StartWith(ConfigurationSettings.CDNAddress);
        }

        internal BookAssert ShouldHaveCorrectLocationHeader()
        {
            var response = _response as CreatedResult;
            response.Location.Should().NotBeNull();
            response.Location.Should().EndWith($"library/{_libraryId}/books/{_book.Id}");
            return this;
        }

        public BookAssert ShouldHaveSavedBook(IDbConnection dbConnection)
        {
            var dbbook = dbConnection.GetBookById(_book.Id);
            return ShouldBeSameAs(dbbook);
        }

        internal BookAssert ShouldBeSameAs(BookDto expected)
        {
            _book.Should().NotBeNull();
            _book.Title.Should().Be(expected.Title);
            _book.Description.Should().Be(expected.Description);
            _book.Language.Should().Be((int)expected.Language);

            _book.IsPublic.Should().Be(expected.IsPublic);
            _book.IsPublished.Should().Be(expected.IsPublished);
            _book.Copyrights.Should().Be((int)expected.Copyrights);
            _book.DateAdded.Should().Be(expected.DateAdded);
            _book.DateUpdated.Should().Be(expected.DateUpdated);
            _book.Status.Should().Be((int)expected.Status);
            _book.YearPublished.Should().Be(expected.YearPublished);
            _book.AuthorId.Should().Be(expected.AuthorId);
            _book.SeriesId.Should().Be(expected.SeriesId);
            _book.SeriesIndex.Should().Be(expected.SeriesIndex);

            return this;
        }
    }

    public static class BookAssertionExtensions
    {
    }
}
