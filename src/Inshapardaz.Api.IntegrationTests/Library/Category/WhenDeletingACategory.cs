using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Category
{
    [TestFixture]
    public class WhenDeletingACategory : IntegrationTestBase
    {
        private Domain.Entities.Library.Category _view;
        private Domain.Entities.Library.Category _category;
        private readonly Guid _userId = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _category = CategoryDataHelper.Create("category1");

            Response = await GetAdminClient(_userId).DeleteAsync($"api/categories/{_category.Id}");

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
        public void ShouldHaveDeletedTheTranslation()
        {
            _view.ShouldBeNull();
        }
    }
}