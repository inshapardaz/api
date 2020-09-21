using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.UpdateCategory
{
    [TestFixture(Permission.Reader)]
    [TestFixture(Permission.Writer)]
    public class WhenUpdatingCategoryWithoutPermission : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUpdatingCategoryWithoutPermission(Permission permission) : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var category = new CategoryView { Id = Random.Number, Name = Random.Name };

            _response = await Client.PutObject($"/library/{LibraryId}/categories/{category.Id}", category);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}
