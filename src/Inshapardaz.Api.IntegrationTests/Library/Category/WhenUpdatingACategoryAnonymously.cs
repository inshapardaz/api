using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View.Library;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Category
{
    [TestFixture]
    public class WhenUpdatingACategoryAnonymously : IntegrationTestBase
    {
        private Domain.Entities.Library.Category _category;
        private readonly Guid _userId = Guid.NewGuid();
        private CategoryView _updatedCategory;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _category = CategoryDataHelper.Create("category1");

            _updatedCategory = new CategoryView {Id = _category.Id, Name = "Some New Name"};

            Response = await GetClient().PutJson($"api/categories/{_category.Id}", _updatedCategory);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            CategoryDataHelper.Delete(_category.Id);
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}