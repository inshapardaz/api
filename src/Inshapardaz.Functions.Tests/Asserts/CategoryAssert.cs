using FluentAssertions;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Views.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Inshapardaz.Functions.Tests.Asserts
{
    public class CategoryAssert
    {
        private CategoryView _category;

        public CategoryAssert(CategoryView view)
        {
            _category = view;
        }

        internal static void ShouldNotHaveDeletedCategory(int categoryId, IDbConnection dbConnection)
        {
            var author = dbConnection.GetCategoryById(categoryId);
            author.Should().NotBeNull();
        }

        internal static void ShouldHaveDeletedCategory(int categoryId, IDbConnection dbConnection)
        {
            var author = dbConnection.GetCategoryById(categoryId);
            author.Should().BeNull();
        }

        public static CategoryAssert FromObject(CategoryView view)
        {
            return new CategoryAssert(view);
        }

        public CategoryAssert ShouldBeSameAs(CategoryDto dto)
        {
            _category.Id.Should().Be(dto.Id);
            _category.Name.Should().Be(dto.Name);
            return this;
        }

        public CategoryAssert ShouldBeSameAs(CategoryView expected)
        {
            _category.Id.Should().Be(expected.Id);
            _category.Name.Should().Be(expected.Name);
            _category.Links.Should().BeEquivalentTo(expected.Links);
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
