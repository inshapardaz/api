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
    public class WhenUpdatingCategoryToSetParent(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private CategoryDto _parent;
        private CategoryDto _category;
        private CategoryAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var categories = CategoryBuilder.WithLibrary(LibraryId).Build(2).ToList();
            _parent = categories[0];
            _category = categories[1];

            var update = new CategoryView { Id = _category.Id, Name = RandomData.Name, ParentCategoryId = _parent.Id };

            _response = await Client.PutObject($"/libraries/{LibraryId}/categories/{_category.Id}", update);
            _assert = Services.GetService<CategoryAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveParentInResponse() => _assert.ShouldHaveParent(_parent.Id);

        [Test]
        public void ShouldHaveSavedParentInDataStore() => _assert.ShouldHaveParentInDataStore(_parent.Id);
    }
}
