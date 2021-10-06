using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using System.Data;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Asserts
{
    public class LibraryAssert
    {
        private HttpResponseMessage _response;
        private LibraryView _view;
        private int _libraryId;

        public LibraryAssert(HttpResponseMessage response, int libraryId)
        {
            _response = response;
            _view = _response.GetContent<LibraryView>().Result;
            _libraryId = libraryId;
        }

        public LibraryAssert(LibraryView view, int libraryId)
        {
            _libraryId = libraryId;
            _view = view;
        }

        public static LibraryAssert FromResponse(HttpResponseMessage response, IDbConnection databaseConnection)
        {
            var view = response.GetContent<LibraryView>().Result;
            return new LibraryAssert(response, databaseConnection.GetLibrary(view).Id);
        }

        public static LibraryAssert FromResponse(HttpResponseMessage response, int libraryId)
        {
            return new LibraryAssert(response, libraryId);
        }

        internal static void ShouldHaveDeletedLibrary(int libraryId, IDbConnection databaseConnection)
        {
            var dbLibrary = databaseConnection.GetLibraryById(libraryId);
            dbLibrary.Should().BeNull();
        }

        internal LibraryAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"libraries/{_libraryId}");
            return this;
        }

        internal LibraryAssert ShouldHaveCreatedLibrary(IDbConnection databaseConnection)
        {
            var library = databaseConnection.GetLibrary(_view);
            library.Name.Should().Be(_view.Name);
            library.Language.Should().Be(_view.Language);
            library.SupportsPeriodicals.Should().Be(_view.SupportsPeriodicals);
            library.PrimaryColor.Should().Be(_view.PrimaryColor);
            library.SecondaryColor.Should().Be(_view.SecondaryColor);
            library.OwnerEmail.Should().Be(_view.OwnerEmail);
            return this;
        }

        internal LibraryAssert WithWritableLinks()
        {
            return ShouldHaveUpdateLink()
                .ShouldHaveDeleteLink();
        }

        internal void ShouldHaveUpdatedLibrary(IDbConnection databaseConnection)
        {
            var dbLibrary = databaseConnection.GetLibraryById(_libraryId);
            _view.Name.Should().Be(dbLibrary.Name);
            _view.Language.Should().Be(dbLibrary.Language);
            _view.SupportsPeriodicals.Should().Be(dbLibrary.SupportsPeriodicals);
            _view.PrimaryColor.Should().Be(dbLibrary.PrimaryColor);
            _view.SecondaryColor.Should().Be(dbLibrary.SecondaryColor);
        }

        public void ShouldBeSameAs(LibraryView expectedLibrary)
        {
            _view.Name.Should().Be(expectedLibrary.Name);
            _view.Language.Should().Be(expectedLibrary.Language);
            _view.SupportsPeriodicals.Should().Be(expectedLibrary.SupportsPeriodicals);
            _view.PrimaryColor.Should().Be(expectedLibrary.PrimaryColor);
            _view.SecondaryColor.Should().Be(expectedLibrary.SecondaryColor);
        }

        public LibraryAssert ShouldBeSameAs(LibraryDto expectedLibrary)
        {
            _view.Name.Should().Be(expectedLibrary.Name);
            _view.Language.Should().Be(expectedLibrary.Language);
            _view.SupportsPeriodicals.Should().Be(expectedLibrary.SupportsPeriodicals);
            _view.PrimaryColor.Should().Be(expectedLibrary.PrimaryColor);
            _view.SecondaryColor.Should().Be(expectedLibrary.SecondaryColor);

            return this;
        }

        public LibraryAssert ShouldHaveSelfLink()
        {
            _view.Links.AssertLink("self")
                       .ShouldBeGet()
                       .EndingWith($"/libraries/{_libraryId}");
            return this;
        }

        public LibraryAssert ShouldHaveBooksLink()
        {
            _view.Links.AssertLink("books")
                       .ShouldBeGet()
                       .EndingWith($"/libraries/{_libraryId}/books");
            return this;
        }

        public LibraryAssert ShouldHaveAuthorsLink()
        {
            _view.Links.AssertLink("authors")
                       .ShouldBeGet()
                       .EndingWith($"/libraries/{_libraryId}/authors");
            return this;
        }

        public LibraryAssert ShouldHaveCategoriesLink()
        {
            _view.Links.AssertLink("categories")
                       .ShouldBeGet()
                       .EndingWith($"/libraries/{_libraryId}/categories");
            return this;
        }

        public LibraryAssert ShouldHaveSeriesLink()
        {
            _view.Links.AssertLink("series")
                       .ShouldBeGet()
                       .EndingWith($"/libraries/{_libraryId}/series");
            return this;
        }

        public LibraryAssert ShouldHavePeriodicalLink()
        {
            _view.Links.AssertLink("periodicals")
                   .ShouldBeGet()
                   .EndingWith($"/libraries/{_libraryId}/periodicals");
            return this;
        }

        public LibraryAssert ShouldNotHavePeriodicalLink()
        {
            _view.Links.AssertLinkNotPresent("periodicals");
            return this;
        }

        public LibraryAssert ShouldHaveRecentLinks()
        {
            _view.Links.AssertLink("recents")
                .ShouldBeGet()
                .EndingWith($"/libraries/{_libraryId}/books")
                .ShouldHaveQueryParameter("read", bool.TrueString);
            return this;
        }

        public LibraryAssert ShouldNotHaveRecentLinks()
        {
            _view.Links.AssertLinkNotPresent("recents");

            return this;
        }

        public LibraryAssert ShouldHaveCreateCategorylink()
        {
            _view.Links.AssertLink("create-category")
                      .ShouldBePost()
                      .EndingWith($"/libraries/{_libraryId}/categories");
            return this;
        }

        public LibraryAssert ShouldHaveUpdateLink()
        {
            _view.Links.AssertLink("update")
                .ShouldBePut()
                .EndingWith($"/libraries/{_libraryId}");
            return this;
        }

        public LibraryAssert ShouldHaveCreateBookLink()
        {
            _view.Links.AssertLink("create-book")
                .ShouldBePost()
                .EndingWith($"/libraries/{_libraryId}/books");
            return this;
        }

        public LibraryAssert ShouldHaveCreateSeriesLink()
        {
            _view.Links.AssertLink("create-series")
                .ShouldBePost()
                .EndingWith($"/libraries/{_libraryId}/series");
            return this;
        }

        public LibraryAssert ShouldHaveCreateAuthorLink()
        {
            _view.Links.AssertLink("create-author")
                .ShouldBePost()
                .EndingWith($"/libraries/{_libraryId}/authors");
            return this;
        }

        public LibraryAssert ShouldHaveCreateCategoryLink()
        {
            _view.Links.AssertLink("create-category")
                .ShouldBePost()
                .EndingWith($"/libraries/{_libraryId}/categories");
            return this;
        }

        public LibraryAssert ShouldNotHaveUpdatelink()
        {
            _view.Links.AssertLinkNotPresent("update");
            return this;
        }

        public LibraryAssert ShouldHaveDeleteLink()
        {
            _view.Links.AssertLink("delete")
                        .ShouldBeDelete()
                        .EndingWith($"/libraries/{_libraryId}");
            return this;
        }

        public LibraryAssert ShouldNotHaveDeletelink()
        {
            _view.Links.AssertLinkNotPresent("delete");
            return this;
        }

        public LibraryAssert ShouldNotHaveCreateBookLink()
        {
            _view.Links.AssertLinkNotPresent("create-book");
            return this;
        }

        public LibraryAssert ShouldNotHaveCreateAuthorLink()
        {
            _view.Links.AssertLinkNotPresent("create-author");
            return this;
        }

        public LibraryAssert ShouldNotHaveCreateSeriesLink()
        {
            _view.Links.AssertLinkNotPresent("create-series");

            return this;
        }

        public LibraryAssert ShouldNotHaveCreateCategoryLink()
        {
            _view.Links.AssertLinkNotPresent("create-category");

            return this;
        }

        public LibraryAssert ShouldNotHaveEditLinks()
        {
            _view.Links.AssertLinkNotPresent("update");
            _view.Links.AssertLinkNotPresent("delete");

            return this;
        }

        internal static LibraryAssert FromObject(LibraryView view, int libraryId)
        {
            return new LibraryAssert(view, libraryId);
        }
    }

    internal static class LibraryAssertionExtensions
    {
        internal static LibraryAssert ShouldMatch(this LibraryView view, LibraryDto dto)
        {
            return LibraryAssert.FromObject(view, dto.Id)
                               .ShouldBeSameAs(dto);
        }
    }
}
