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
    public class WhenUpdatingACategory : IntegrationTestBase
    {
        private Domain.Entities.Library.Category _view;
        private Domain.Entities.Library.Category _category;
        private readonly Guid _userId = Guid.NewGuid();
        private CategoryView _updatedCategory;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _category = CategoryDataHelper.Create("category1");

            _updatedCategory = new CategoryView {Id = _category.Id, Name = "Some New Name"};

            Response = await GetAdminClient(_userId).PutJson($"api/categories/{_category.Id}", _updatedCategory);

            _view = CategoryDataHelper.Get(_category.Id);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            CategoryDataHelper.Delete(_category.Id);
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Test]
        public void ShouldHaveUpdateTheTranslation()
        {
            _view.Name.ShouldBe(_updatedCategory.Name);
        }
    }
}