using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.UpdateCategory
{
    public class WhenUpdatingCategoryToBeItsOwnParent() : TestBase(Role.LibraryAdmin)
    {
        private HttpResponseMessage _response;
        private CategoryDto _category;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _category = CategoryBuilder.WithLibrary(LibraryId).Build();

            var update = new CategoryView { Id = _category.Id, Name = RandomData.Name, ParentCategoryId = _category.Id };

            _response = await Client.PutObject($"/libraries/{LibraryId}/categories/{_category.Id}", update);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveBadRequestResult() => _response.ShouldBeBadRequest();

        [Test]
        public void ShouldNotHaveSetParent()
        {
            Services.GetService<CategoryAssert>()
                .ForView(_category.ToView())
                .ForLibrary(LibraryId)
                .ShouldHaveParentInDataStore(null);
        }
    }
}
