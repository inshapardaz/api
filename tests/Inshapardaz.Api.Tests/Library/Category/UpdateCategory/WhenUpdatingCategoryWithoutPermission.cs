using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.UpdateCategory
{
    [TestFixture(Role.Reader)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingCategoryWithoutPermission(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var category = new CategoryView { Id = RandomData.Number, Name = RandomData.Name };

            _response = await Client.PutObject($"/libraries/{LibraryId}/categories/{category.Id}", category);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveForbiddenResult() => _response.ShouldBeForbidden();
    }
}
