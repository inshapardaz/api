using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.AddCategory
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenAddingCategoryWithParent(Role role) : TestBase(role)
    {
        private CategoryDto _parent;
        private CategoryView _category;
        private HttpResponseMessage _response;
        private CategoryAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _parent = CategoryBuilder.WithLibrary(LibraryId).Build();
            _category = new CategoryView { Name = RandomData.Name, ParentCategoryId = _parent.Id };

            _response = await Client.PostObject($"/libraries/{LibraryId}/categories", _category);
            _assert = Services.GetService<CategoryAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveCreatedResult() => _response.ShouldBeCreated();

        [Test]
        public void ShouldHaveLocationHeader() => _assert.ShouldHaveCorrectLocationHeader();

        [Test]
        public void ShouldHaveParentInResponse() => _assert.ShouldHaveParent(_parent.Id);

        [Test]
        public void ShouldHaveSavedParentInDataStore() => _assert.ShouldHaveParentInDataStore(_parent.Id);

        [Test]
        public void ShouldHaveParentAndChildrenLinks()
        {
            _assert.ShouldHaveParentLink(_parent.Id)
                   .ShouldHaveChildrenLink();
        }
    }
}
