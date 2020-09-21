using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using System;
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
            location.Should().EndWith($"library/{_libraryId}");
            return this;
        }

        internal LibraryAssert ShouldHaveCreatedLibrary(IDbConnection databaseConnection)
        {
            var library = databaseConnection.GetLibrary(_view);
            library.Name.Should().Be(_view.Name);
            library.Language.Should().Be(_view.Language);
            library.SupportsPeriodicals.Should().Be(_view.SupportsPeriodicals);
            return this;
        }

        internal void ShouldHaveUpdatedLibrary(IDbConnection databaseConnection)
        {
            var dbLibrary = databaseConnection.GetLibraryById(_libraryId);
            _view.Name.Should().Be(dbLibrary.Name);
            _view.Language.Should().Be(dbLibrary.Language);
            _view.SupportsPeriodicals.Should().Be(dbLibrary.SupportsPeriodicals);
        }

        internal void ShouldBeSameAs(LibraryView expectedLibrary)
        {
            _view.Name.Should().Be(expectedLibrary.Name);
            _view.Language.Should().Be(expectedLibrary.Language);
            _view.SupportsPeriodicals.Should().Be(expectedLibrary.SupportsPeriodicals);
        }

        public LibraryAssert ShouldHaveSelfLink()
        {
            _view.Links.AssertLink("self")
                       .ShouldBeGet()
                       .EndingWith($"/library/{_libraryId}");
            return this;
        }

        public LibraryAssert ShouldHaveBooksLink()
        {
            _view.Links.AssertLink("books")
                       .ShouldBeGet()
                       .EndingWith($"/library/{_libraryId}/books");
            return this;
        }

        public LibraryAssert ShouldHaveAuthorsLink()
        {
            _view.Links.AssertLink("authors")
                       .ShouldBeGet()
                       .EndingWith($"/library/{_libraryId}/authors");
            return this;
        }

        public LibraryAssert ShouldHaveCategoriesLink()
        {
            _view.Links.AssertLink("categories")
                       .ShouldBeGet()
                       .EndingWith($"/library/{_libraryId}/categories");
            return this;
        }

        public LibraryAssert ShouldHaveSeriesLink()
        {
            _view.Links.AssertLink("series")
                       .ShouldBeGet()
                       .EndingWith($"/library/{_libraryId}/series");
            return this;
        }

        public LibraryAssert ShouldHavePeriodicalLink()
        {
            _view.Links.AssertLink("periodicals")
                   .ShouldBeGet()
                   .EndingWith($"/library/{_libraryId}/periodicals");
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
                .EndingWith($"/library/{_libraryId}/books")
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
                      .EndingWith($"/library/{_libraryId}/categories");
            return this;
        }

        public LibraryAssert ShouldNotHaveCreateCategorylink()
        {
            _view.Links.AssertLinkNotPresent("create-category");
            return this;
        }

        public LibraryAssert ShouldHaveCreatelink()
        {
            _view.Links.AssertLink("create")
                        .ShouldBePost()
                        .EndingWith($"/library");
            return this;
        }

        public LibraryAssert ShouldNotHaveCreatelink()
        {
            _view.Links.AssertLinkNotPresent("create");
            return this;
        }

        public LibraryAssert ShouldHaveUpdatLlink()
        {
            _view.Links.AssertLink("update")
                .ShouldBePut()
                .EndingWith($"/library/{_libraryId}");
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
                        .EndingWith($"/library/{_libraryId}");
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

        public LibraryAssert ShouldNotHaveSerieslink()
        {
            _view.Links.AssertLinkNotPresent("create-series");
            return this;
        }

        public LibraryAssert ShouldNotHaveCreateAuthorLink()
        {
            _view.Links.AssertLinkNotPresent("create-author");
            return this;
        }

        public LibraryAssert ShouldNotHaveEditLinks()
        {
            _view.Links.AssertLinkNotPresent("create-book");
            _view.Links.AssertLinkNotPresent("create-category");
            _view.Links.AssertLinkNotPresent("create-series");
            _view.Links.AssertLinkNotPresent("create-author");
            _view.Links.AssertLinkNotPresent("create");
            _view.Links.AssertLinkNotPresent("update");
            _view.Links.AssertLinkNotPresent("delete");

            return this;
        }
    }
}
