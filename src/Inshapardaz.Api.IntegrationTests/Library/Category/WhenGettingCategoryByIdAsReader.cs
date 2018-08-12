using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Category
{
    [TestFixture, Ignore("Security not implemented")]
    public class WhenGettingCategoryByIdAsReader : IntegrationTestBase
    {
        private CategoryView _view;
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Library.Category _category;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _category = CategoryDataHelper.Create("Category2323");

            Response = await GetReaderClient(_userId).GetAsync($"/api/categories/{_category.Id}");
            _view = JsonConvert.DeserializeObject<CategoryView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(_category.Id);
        }

        [Test]
        public void ShouldReturnOk()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldReturnCategoryWithCorrectId()
        {
            _view.Id.ShouldBe(_view.Id);
        }

        [Test]
        public void ShouldReturnCategoryWithCorrectName()
        {
            _view.Name.ShouldBe(_category.Name);
        }

        [Test]
        public void ShouldReturnSelfLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Self);
        }

        [Test]
        public void ShouldNotReturnUpdateLink()
        {
            _view.Links.ShouldNotContain(link => link.Rel == RelTypes.Update);
        }

        [Test]
        public void ShouldNotReturnDeleteLink()
        {
            _view.Links.ShouldNotContain(link => link.Rel == RelTypes.Delete);
        }
    }
}