using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.DeleteCategory
{
    public class WhenDeletingCategoryWithChildren() : TestBase(Role.LibraryAdmin)
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

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/categories/{_parent.Id}");
            _assert = Services.GetService<CategoryAssert>().ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveBadRequestResult() => _response.ShouldBeBadRequest();

        [Test]
        public void ShouldNotHaveDeletedParent() => _assert.ShouldNotHaveDeletedCategory(_parent.Id);

        [Test]
        public void ShouldNotHaveDeletedChild() => _assert.ShouldNotHaveDeletedCategory(_child.Id);
    }
}
