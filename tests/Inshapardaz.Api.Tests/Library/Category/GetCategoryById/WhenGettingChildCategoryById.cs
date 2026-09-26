using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.GetCategoryById
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.Reader)]
    public class WhenGettingChildCategoryById(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private CategoryAssert _assert;
        private CategoryDto _parent;
        private CategoryDto _child;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _parent = CategoryBuilder.WithLibrary(LibraryId).Build();
            _child = CategoryBuilder.WithParent(_parent).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/categories/{_child.Id}");
            _assert = Services.GetService<CategoryAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnCategoryWithParent()
        {
            _assert.ShouldBeSameAs(_child)
                   .ShouldHaveParent(_parent.Id)
                   .ShouldHaveParentLink(_parent.Id)
                   .ShouldHaveChildrenLink()
                   .ShouldHaveSelfLink();
        }
    }
}
