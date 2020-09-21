using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.DeleteCategory
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    public class WhenDeletingCategoryWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private IEnumerable<CategoryDto> _categories;
        private CategoryDto _selectedCategory;

        public WhenDeletingCategoryWithPermission(Permission permission) : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).Build(4);
            _selectedCategory = _categories.PickRandom();

            _response = await Client.DeleteAsync($"/library/{LibraryId}/categories/{_selectedCategory.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNoContentResult()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedCategory()
        {
            CategoryAssert.ShouldHaveDeletedCategory(LibraryId, _selectedCategory.Id, DatabaseConnection);
        }
    }
}
