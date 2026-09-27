using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.AddCategory
{
    public class WhenAddingCategoryWithNonExistingParent() : TestBase(Role.LibraryAdmin)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var category = new CategoryView { Name = RandomData.Name, ParentCategoryId = -RandomData.Number };

            _response = await Client.PostObject($"/libraries/{LibraryId}/categories", category);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveBadRequestResult() => _response.ShouldBeBadRequest();
    }
}
