using FluentAssertions;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Adapters;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class LibraryAssert
    {
        private HttpResponseMessage _response;
        private LibraryView _view;
        private int _libraryId;
        private readonly ILibraryTestRepository _libraryRepository;
        private readonly IFileTestRepository _fileRepository;
        private readonly IAuthorTestRepository _authorRepository;
        private readonly ICategoryTestRepository _categoryRepository;
        private readonly ISeriesTestRepository _seriesRepository;
        private readonly FakeFileStorage _fileStorage;

        public LibraryAssert(ILibraryTestRepository libraryRepository,
            IFileTestRepository fileRepository,
            IAuthorTestRepository authorRepository,
            ICategoryTestRepository categoryRepository,
            FakeFileStorage fileStorage,
            ISeriesTestRepository seriesRepository)
        {
            _libraryRepository = libraryRepository;
            _fileRepository = fileRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _fileStorage = fileStorage;
            _seriesRepository = seriesRepository;
        }
        public LibraryAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _view = _response.GetContent<LibraryView>().Result;
            return this;
        }

        public LibraryAssert ForView(LibraryView view)
        {
            _view = view;
            return this;
        }

        public LibraryAssert ForLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }


        public LibraryAssert ShouldHaveDeletedLibrary(int libraryId)
        {
            var dbLibrary = _libraryRepository.GetLibraryById(libraryId);
            dbLibrary.Should().BeNull();
            return this;
        }

        public LibraryAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"libraries/{_libraryId}");
            return this;
        }

        public LibraryAssert ShouldHaveCreatedLibrary()
        {
            var library = _libraryRepository.GetLibrary(_view);
            library.Name.Should().Be(_view.Name);
            library.Language.Should().Be(_view.Language);
            library.SupportsPeriodicals.Should().Be(_view.SupportsPeriodicals);
            library.PrimaryColor.Should().Be(_view.PrimaryColor);
            library.SecondaryColor.Should().Be(_view.SecondaryColor);
            library.OwnerEmail.Should().Be(_view.OwnerEmail);
            library.DatabaseConnection.Should().BeNull();
            library.FileStoreType.ToEnum<FileStoreTypes>(FileStoreTypes.Unknown).ToDescription().Should().Be(_view.FileStoreType);
            library.FileStoreSource.Should().Be(_view.FileStoreSource);
            return this;
        }

        public LibraryAssert ShouldHaveCreatedLibraryWithConfiguration()
        {
            var library = _libraryRepository.GetLibrary(_view);
            library.Name.Should().Be(_view.Name);
            library.Language.Should().Be(_view.Language);
            library.SupportsPeriodicals.Should().Be(_view.SupportsPeriodicals);
            library.PrimaryColor.Should().Be(_view.PrimaryColor);
            library.SecondaryColor.Should().Be(_view.SecondaryColor);
            library.OwnerEmail.Should().Be(_view.OwnerEmail);
            library.DatabaseConnection.Should().NotBeNull();
            library.FileStoreType.ToEnum<FileStoreTypes>(FileStoreTypes.Unknown).ToDescription().Should().Be(_view.FileStoreType);
            library.FileStoreSource.Should().Be(_view.FileStoreSource);
            return this;
        }

        public LibraryAssert WithWritableLinks()
        {
            return ShouldHaveUpdateLink()
                .ShouldHaveDeleteLink();
        }

        public LibraryAssert ShouldHaveUpdatedLibrary()
        {
            var dbLibrary = _libraryRepository.GetLibraryById(_libraryId);
            _view.Name.Should().Be(dbLibrary.Name);
            _view.Language.Should().Be(dbLibrary.Language);
            _view.SupportsPeriodicals.Should().Be(dbLibrary.SupportsPeriodicals);
            _view.PrimaryColor.Should().Be(dbLibrary.PrimaryColor);
            _view.SecondaryColor.Should().Be(dbLibrary.SecondaryColor);
            _view.FileStoreType.ToEnum<FileStoreTypes>(FileStoreTypes.Unknown).Should().Be(dbLibrary.FileStoreType.ToEnum<FileStoreTypes>(FileStoreTypes.Unknown));
            _view.FileStoreSource.Should().Be(dbLibrary.FileStoreSource);
            return this;
        }

        public LibraryAssert ShouldHaveUpdatedLibraryWithoutConfiguration()
        {
            var dbLibrary = _libraryRepository.GetLibraryById(_libraryId);
            _view.Name.Should().Be(dbLibrary.Name);
            _view.Language.Should().Be(dbLibrary.Language);
            _view.SupportsPeriodicals.Should().Be(dbLibrary.SupportsPeriodicals);
            _view.PrimaryColor.Should().Be(dbLibrary.PrimaryColor);
            _view.SecondaryColor.Should().Be(dbLibrary.SecondaryColor);
            return this;
        }

        public LibraryAssert ShouldBeSameAs(LibraryView expectedLibrary)
        {
            _view.Name.Should().Be(expectedLibrary.Name);
            _view.Language.Should().Be(expectedLibrary.Language);
            _view.SupportsPeriodicals.Should().Be(expectedLibrary.SupportsPeriodicals);
            _view.PrimaryColor.Should().Be(expectedLibrary.PrimaryColor);
            _view.SecondaryColor.Should().Be(expectedLibrary.SecondaryColor);
            _view.DatabaseConnection.Should().Be(expectedLibrary.DatabaseConnection);
            _view.FileStoreType.Should().Be(expectedLibrary.FileStoreType);
            _view.FileStoreSource.Should().Be(expectedLibrary.FileStoreSource);
            return this;
        }

        public LibraryAssert ShouldBeSameWithNoConfiguration(LibraryView expectedLibrary)
        {
            _view.Name.Should().Be(expectedLibrary.Name);
            _view.Language.Should().Be(expectedLibrary.Language);
            _view.SupportsPeriodicals.Should().Be(expectedLibrary.SupportsPeriodicals);
            _view.PrimaryColor.Should().Be(expectedLibrary.PrimaryColor);
            _view.SecondaryColor.Should().Be(expectedLibrary.SecondaryColor);
            _view.DatabaseConnection.Should().BeNull();
            _view.FileStoreType.Should().BeNull();
            _view.FileStoreSource.Should().BeNull();
            return this;
        }

        public LibraryAssert ShouldBeSameAs(LibraryDto expectedLibrary)
        {
            _view.Name.Should().Be(expectedLibrary.Name);
            _view.Language.Should().Be(expectedLibrary.Language);
            _view.SupportsPeriodicals.Should().Be(expectedLibrary.SupportsPeriodicals);
            _view.PrimaryColor.Should().Be(expectedLibrary.PrimaryColor);
            _view.SecondaryColor.Should().Be(expectedLibrary.SecondaryColor);
            _view.FileStoreType.Should().Be(expectedLibrary.FileStoreType);
            _view.FileStoreSource.Should().Be(expectedLibrary.FileStoreSource);

            return this;
        }

        public LibraryAssert ShouldBeSameWithNoConfiguration(LibraryDto expectedLibrary)
        {
            _view.Name.Should().Be(expectedLibrary.Name);
            _view.Language.Should().Be(expectedLibrary.Language);
            _view.SupportsPeriodicals.Should().Be(expectedLibrary.SupportsPeriodicals);
            _view.PrimaryColor.Should().Be(expectedLibrary.PrimaryColor);
            _view.SecondaryColor.Should().Be(expectedLibrary.SecondaryColor);
            _view.FileStoreType.Should().BeNull();
            _view.FileStoreSource.Should().BeNull();

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

        public LibraryAssert ShouldNotHaveUpdatedLibraryImage(int libraryId, byte[] newImage)
        {
            var imageUrl = _libraryRepository.GetLibraryImageUrl(libraryId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotEqual(newImage);
            return this;
        }

        public LibraryAssert ShouldHaveCorrectImageLocationHeader(int libraryId)
        {
            _response.Headers.Location.Should().NotBeNull();
            return this;
        }

        public LibraryAssert ShouldHaveAddedLibraryImage(int libraryId, byte[] newImage)
        {
            var imageUrl = _libraryRepository.GetLibraryImageUrl(libraryId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty().And.Equal(newImage);
            return this;
        }

        public LibraryAssert ShouldHavePublicImage(int libraryId)
        {
            var image = _libraryRepository.GetLibraryImage(libraryId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
            return this;
        }

        public LibraryAssert ShouldHaveUpdatedImage(int libraryId, byte[] newImage)
        {
            var imageUrl = _libraryRepository.GetLibraryImageUrl(libraryId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.Equal(newImage);
            return this;
        }

    }
}
