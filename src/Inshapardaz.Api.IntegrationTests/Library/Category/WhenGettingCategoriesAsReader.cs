using System;
using System.Collections.Generic;
using System.Linq;
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
    public class WhenGettingCategoriesAsReader : IntegrationTestBase
    {
        private readonly List<Domain.Entities.Library.Category> _categories = new List<Domain.Entities.Library.Category>();
        private ListView<CategoryView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories.Add(CategoryDataHelper.Create("category 1"));
            _categories.Add(CategoryDataHelper.Create("category 2"));
            _categories.Add(CategoryDataHelper.Create("category 3"));
            _categories.Add(CategoryDataHelper.Create("category 4"));

             Response = await GetReaderClient(Guid.NewGuid()).GetAsync($"/api/categories/");
            _view = JsonConvert.DeserializeObject<ListView<CategoryView>>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            foreach (var category in _categories)
            {
                CategoryDataHelper.Delete(category.Id);
            }
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldContainAllCategories()
        {
            _view.Items.Count().ShouldBe(_categories.Count);
        }

        [Test]
        public void ShouldReturnSelfLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Self);
        }

        [Test]
        public void ShouldNotReturnCreateLink()
        {
            _view.Links.ShouldNotContain(link => link.Rel == RelTypes.Create);
        }
    }
}