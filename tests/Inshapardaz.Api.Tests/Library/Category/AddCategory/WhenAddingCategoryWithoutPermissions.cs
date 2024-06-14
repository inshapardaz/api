using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.AddCategory
{
    [TestFixture(Role.Reader)]
    [TestFixture(Role.Writer)]
    public class WhenAddingCategoryWithoutPermissions : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingCategoryWithoutPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var category = new CategoryView { Name = new Faker().Random.String() };

            _response = await Client.PostObject($"/libraries/{LibraryId}/categories", category);
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