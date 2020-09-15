using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Api.Tests;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.AddCategory
{
    [TestFixture(Permission.Reader)]
    [TestFixture(Permission.Writer)]
    public class WhenAddingCategoryWithoutPermissions : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingCategoryWithoutPermissions(Permission permission)
            : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var category = new CategoryView { Name = new Faker().Random.String() };

            _response = await Client.PostObject($"/library/{LibraryId}/categories", category);
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
