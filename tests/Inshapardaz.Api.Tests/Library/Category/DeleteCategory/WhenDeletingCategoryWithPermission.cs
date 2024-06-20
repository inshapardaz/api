using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.DeleteCategory
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenDeletingCategoryWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private CategoryAssert _assert;
        private IEnumerable<CategoryDto> _categories;
        private CategoryDto _selectedCategory;

        public WhenDeletingCategoryWithPermission(Role role) : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).Build(4);
            _selectedCategory = _categories.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/categories/{_selectedCategory.Id}");
            _assert = Services.GetService<CategoryAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _assert.ShouldHaveDeletedCategory(_selectedCategory.Id);
        }
    }
}
