using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.GetCategoryById
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenGettingCategoryWithWritePermissions : TestBase
    {
        private HttpResponseMessage _response;
        private IEnumerable<CategoryDto> _categories;
        private CategoryDto _selectedCategory;
        private CategoryAssert _assert;

        public WhenGettingCategoryWithWritePermissions(Role role) : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);
            _selectedCategory = _categories.First();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/categories/{_selectedCategory.Id}");

            _assert = Services.GetService<CategoryAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldNotEditLinks()
        {
            _assert.ShouldHaveUpdateLink();
            _assert.ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveBooksLink()
        {
            _assert.ShouldHaveBooksLink();
        }

        [Test]
        public void ShouldReturnCorrectCategoryData()
        {
            _assert.ShouldBeSameAs(_selectedCategory);
        }
    }
}
