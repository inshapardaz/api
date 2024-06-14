using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.DeleteCategory
{
    [TestFixture]
    public class WhenDeletingLibraryAsAnonymous : TestBase
    {
        private HttpResponseMessage _response;

        private IEnumerable<CategoryDto> _categories;
        private CategoryDto _selectedCategory;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).Build(4);
            _selectedCategory = _categories.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/categories/{_selectedCategory.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
