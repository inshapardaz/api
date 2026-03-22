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
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenGettingCategoriesWithWritePermissions(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private IEnumerable<CategoryDto> _categories;
        private ListView<CategoryView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/categories");

            _view = await _response.GetContent<ListView<CategoryView>>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.SelfLink()
                .ShouldBeGet()
                .EndingWith($"/libraries/{LibraryId}/categories");
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            _view.CreateLink()
                .ShouldBePost()
                .EndingWith($"/libraries/{LibraryId}/categories");
        }

        [Test]
        public void ShouldHaveSomeCategories()
        {
            foreach (var item in _categories)
            {
                var actual = _view.Data.FirstOrDefault(x => x.Id == item.Id);
                Services.GetService<CategoryAssert>().ForView(actual)
                    .ForLibrary(LibraryId)
                    .ShouldBeSameAs(item)
                    .WithBookCount(3)
                    .ShouldHaveUpdateLink()
                    .ShouldHaveDeleteLink();
            }
        }
    }
}
