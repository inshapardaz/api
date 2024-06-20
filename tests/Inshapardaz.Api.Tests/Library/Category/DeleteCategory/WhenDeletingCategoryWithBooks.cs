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
    public class WhenDeletingCategoryWithBooks : TestBase
    {
        private HttpResponseMessage _response;
        private CategoryAssert _assert;
        private IEnumerable<CategoryDto> _categories;
        private CategoryDto _selectedCategory;

        public WhenDeletingCategoryWithBooks() 
            : base(Role.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);
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
