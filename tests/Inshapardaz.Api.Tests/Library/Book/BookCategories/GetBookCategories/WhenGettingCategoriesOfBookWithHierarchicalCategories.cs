using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.BookCategories.GetBookCategories
{
    public class WhenGettingCategoriesOfBookWithHierarchicalCategories() : TestBase(Role.Reader)
    {
        private ListView<CategoryView> _view;
        private CategoryDto _parent;
        private CategoryDto _child;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _parent = CategoryBuilder.WithLibrary(LibraryId).Build();
            _child = CategoryBuilder.WithParent(_parent).Build();
            var book = BookBuilder.WithLibrary(LibraryId).IsPublic().WithCategories(new[] { _child }).Build();

            var response = await Client.GetAsync($"/libraries/{LibraryId}/books/{book.Id}/categories");
            _view = await response.GetContent<ListView<CategoryView>>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnAssignedCategoryWithItsParent()
        {
            var category = _view.Data.Should().ContainSingle().Subject;
            category.Id.Should().Be(_child.Id);
            category.ParentCategoryId.Should().Be(_parent.Id);
        }
    }
}
