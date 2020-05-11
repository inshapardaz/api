using FluentAssertions;
using Inshapardaz.Functions.Tests.DataHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Inshapardaz.Functions.Tests.Asserts
{
    internal class CategoryAssert
    {
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
    }
}
