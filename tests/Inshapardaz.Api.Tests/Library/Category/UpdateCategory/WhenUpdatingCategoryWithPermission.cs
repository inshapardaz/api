using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.UpdateCategory
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenUpdatingCategoryWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private IEnumerable<CategoryDto> _categories;
        private CategoryDto _selectedCategory;
        private CategoryView _expectedCategory;
        private CategoryAssert _assert;

        public WhenUpdatingCategoryWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);

            _selectedCategory = _categories.PickRandom();

            _expectedCategory = new CategoryView { Id = _selectedCategory.Id, Name = RandomData.Name };

            _response = await Client.PutObject($"/libraries/{LibraryId}/categories/{_selectedCategory.Id}", _expectedCategory);
            _assert = Services.GetService<CategoryAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedTheCategory()
        {
            _assert.ShouldBeSameAs(_expectedCategory);
        }

        [Test]
        public void ShouldHaveUpdatedCategory()
        {
            _assert.ShouldHaveUpdatedCategory();
        }
    }
}
