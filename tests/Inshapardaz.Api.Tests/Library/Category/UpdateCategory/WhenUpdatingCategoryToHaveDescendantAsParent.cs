using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.UpdateCategory
{
    public class WhenUpdatingCategoryToHaveDescendantAsParent() : TestBase(Role.LibraryAdmin)
    {
        private HttpResponseMessage _response;
        private CategoryDto _root;
        private CategoryDto _grandChild;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _root = CategoryBuilder.WithLibrary(LibraryId).Build();
            var child = CategoryBuilder.WithParent(_root).Build();
            _grandChild = CategoryBuilder.WithParent(child).Build();

            var update = new CategoryView { Id = _root.Id, Name = RandomData.Name, ParentCategoryId = _grandChild.Id };

            _response = await Client.PutObject($"/libraries/{LibraryId}/categories/{_root.Id}", update);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveBadRequestResult() => _response.ShouldBeBadRequest();

        [Test]
        public void ShouldNotHaveChangedParent()
        {
            Services.GetService<CategoryAssert>()
                .ForView(_root.ToView())
                .ForLibrary(LibraryId)
                .ShouldHaveParentInDataStore(null);
        }
    }
}
