using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.GetCategoryById
{
    [TestFixture(Permission.Unauthorised)]
    [TestFixture(Permission.Reader)]
    [TestFixture(Permission.Writer)]
    public class WhenGettingCategoryWithoutWritePermissions : TestBase
    {
        private HttpResponseMessage _response;
        private IEnumerable<CategoryDto> _categories;
        private CategoryDto _selectedCategory;
        private CategoryAssert _assert;

        public WhenGettingCategoryWithoutWritePermissions(Permission permission)
            : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);
            _selectedCategory = _categories.PickRandom();

            _response = await Client.GetAsync($"/library/{LibraryId}/categories/{_selectedCategory.Id}");

            _assert = CategoryAssert.FromResponse(_response).InLibrary(LibraryId);
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
            _assert.ShouldNotHaveUpdateLink();
            _assert.ShouldNotHaveDeleteLink();
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
