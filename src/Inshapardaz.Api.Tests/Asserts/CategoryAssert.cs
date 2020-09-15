using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using System.Data;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Asserts
{
    public class CategoryAssert
    {
        private HttpResponseMessage _response;
        private CategoryView _category;
        private int _libraryId;

        public CategoryAssert(CategoryView view)
        {
            _category = view;
        }

        public CategoryAssert(HttpResponseMessage response)
        {
            _response = response;
            _category = response.GetContent<CategoryView>().Result;
        }

        internal static CategoryAssert FromResponse(HttpResponseMessage response)
        {
            return new CategoryAssert(response);
        }

        internal static CategoryAssert FromObject(CategoryView view)
        {
            return new CategoryAssert(view);
        }

        internal static void ShouldNotHaveDeletedCategory(int libraryId, int categoryId, IDbConnection dbConnection)
        {
            var author = dbConnection.GetCategoryById(libraryId, categoryId);
            author.Should().NotBeNull();
        }

        internal static void ShouldHaveDeletedCategory(int libraryId, int categoryId, IDbConnection dbConnection)
        {
            var author = dbConnection.GetCategoryById(libraryId, categoryId);
            author.Should().BeNull();
        }

        internal CategoryAssert ShouldHaveCorrectLocationHeader()
        {
            _response.Headers.Location.Should().NotBeNull();
            _response.Headers.Location.AbsolutePath.Should().EndWith($"library/{_libraryId}/categories/{_category.Id}");
            return this;
        }

        internal CategoryAssert InLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        internal CategoryAssert ShouldHaveBooksLink()
        {
            _category.Link("books")
                  .ShouldBeGet()
                  .EndingWith($"library/{_libraryId}/books")
                  .ShouldHaveQueryParameter("categoryid", _category.Id);

            return this;
        }

        internal CategoryAssert ShouldHaveDeleteLink()
        {
            _category.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"library/{_libraryId}/categories/{_category.Id}");
            return this;
        }

        internal CategoryAssert ShouldNotHaveUpdateLink()
        {
            _category.UpdateLink().Should().BeNull();

            return this;
        }

        internal CategoryAssert ShouldNotHaveDeleteLink()
        {
            _category.DeleteLink().Should().BeNull();
            return this;
        }

        internal CategoryAssert WithReadOnlyLinks()
        {
            ShouldNotHaveDeleteLink();
            ShouldNotHaveUpdateLink();
            return this;
        }

        internal CategoryAssert ShouldHaveUpdatedCategory(IDbConnection dbConnection)
        {
            var dbCat = dbConnection.GetCategoryById(_libraryId, _category.Id);
            dbCat.Should().NotBeNull();
            dbCat.Id.Should().Be(_category.Id);
            dbCat.Name.Should().Be(_category.Name);
            return this;
        }

        internal CategoryAssert WithBookCount(int bookCount)
        {
            _category.BookCount.Should().Be(bookCount);
            return this;
        }

        internal CategoryAssert ShouldHaveUpdateLink()
        {
            _category.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"library/{_libraryId}/categories/{_category.Id}");

            return this;
        }

        internal CategoryAssert ShouldHaveSelfLink(string url)
        {
            _category.SelfLink()
                  .ShouldBeGet()
                  .EndingWith(url);

            return this;
        }

        internal CategoryAssert ShouldHaveSelfLink()
        {
            _category.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"library/{_libraryId}/categories/{_category.Id}");

            return this;
        }

        internal CategoryAssert ShouldHaveCreatedCategory(IDbConnection dbConnection)
        {
            var cat = dbConnection.GetCategoryById(_libraryId, _category.Id);
            cat.Should().NotBeNull();
            return this;
        }

        public CategoryAssert ShouldBeSameAs(CategoryDto dto)
        {
            _category.Id.Should().Be(dto.Id);
            _category.Name.Should().Be(dto.Name);
            return this;
        }

        public CategoryAssert ShouldBeSameAs(CategoryView expected, bool matchLinks = false)
        {
            _category.Id.Should().Be(expected.Id);
            _category.Name.Should().Be(expected.Name);
            if (matchLinks)
            {
                _category.Links.Should().BeEquivalentTo(expected.Links);
            }
            return this;
        }
    }

    public static class CategoryAssertionExtensions
    {
        public static CategoryAssert ShouldMatch(this CategoryView view, CategoryDto dto)
        {
            return CategoryAssert.FromObject(view).ShouldBeSameAs(dto);
        }

        public static CategoryAssert ShouldMatch(this CategoryView view, CategoryView expected)
        {
            return CategoryAssert.FromObject(view).ShouldBeSameAs(expected);
        }
    }
}
