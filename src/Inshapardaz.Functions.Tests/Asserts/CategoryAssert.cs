using FluentAssertions;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Inshapardaz.Functions.Tests.Asserts
{
    public class CategoryAssert
    {
        private ObjectResult _response;
        private CategoryView _category;
        private int _libraryId;

        public CategoryAssert(CategoryView view)
        {
            _category = view;
        }

        public CategoryAssert(ObjectResult response)
        {
            _response = response;
            _category = response.Value as CategoryView;
        }

        internal static CategoryAssert FromResponse(ObjectResult response)
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
            var response = _response as CreatedResult;
            response.Location.Should().NotBeNull();
            response.Location.Should().EndWith($"library/{_libraryId}/categories/{_category.Id}");
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
                  .EndingWith($"library/{_libraryId}/categories/{_category.Id}/books");

            return this;
        }

        internal CategoryAssert ShouldHaveDeleteLink()
        {
            _category.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"library/{_libraryId}/categories/{_category.Id}");
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

        internal CategoryAssert ShouldHaveUpdateLink()
        {
            _category.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"library/{_libraryId}/categories/{_category.Id}");

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
