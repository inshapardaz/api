using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.GetCategories
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenGettingCategoriesWithWritePermissions : TestBase
    {
        private HttpResponseMessage _response;
        private IEnumerable<CategoryDto> _categories;
        private ListView<CategoryView> _view;

        public WhenGettingCategoriesWithWritePermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);

            _response = await Client.GetAsync($"/library/{LibraryId}/categories");

            _view = await _response.GetContent<ListView<CategoryView>>();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.SelfLink()
                .ShouldBeGet()
                .EndingWith($"/library/{LibraryId}/categories");
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            _view.CreateLink()
                .ShouldBePost()
                .EndingWith($"/library/{LibraryId}/categories");
        }

        [Test]
        public void ShouldHaveSomeCategories()
        {
            foreach (var item in _categories)
            {
                var actual = _view.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatch(item)
                      .InLibrary(LibraryId)
                      .WithBookCount(3)
                      .ShouldHaveUpdateLink()
                      .ShouldHaveDeleteLink();
            }
        }
    }
}
