using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.GetCategories
{
    // Existing flat categories (no parent set) must keep behaving exactly as before hierarchy support.
    [TestFixture(Role.Admin)]
    [TestFixture(Role.Reader)]
    public class WhenGettingCategoriesWithoutHierarchy(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private IEnumerable<CategoryDto> _categories;
        private ListView<CategoryView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).WithBooks(2).Build(3);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/categories");
            _view = await _response.GetContent<ListView<CategoryView>>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnAllCategoriesAsFlatListWithNoParents()
        {
            foreach (var item in _categories)
            {
                var actual = _view.Data.Single(x => x.Id == item.Id);
                Services.GetService<CategoryAssert>().ForView(actual)
                    .ForLibrary(LibraryId)
                    .ShouldBeSameAs(item)
                    .ShouldHaveParent(null)
                    .ShouldHaveChildCount(0)
                    .ShouldNotHaveParentLink()
                    .WithBookCount(2);
            }
        }
    }
}
