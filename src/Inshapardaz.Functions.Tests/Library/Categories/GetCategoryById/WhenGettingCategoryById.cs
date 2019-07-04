using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Inshapardaz.Ports.Database.Entities.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.GetCategoryById
{
    [TestFixture]
    public class WhenGettingCategoryById : FunctionTest
    {
        OkObjectResult _response;
        CategoryView _view;
        private IEnumerable<Category> _categories;
        private Category _selectedCategory;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var categoriesBuilder = Container.GetService<CategoriesDataBuilder>();
            _categories = categoriesBuilder.WithCategories(4).Build();
            _selectedCategory = _categories.First();
            
            var handler = Container.GetService<Functions.Library.Categories.GetCategoryById>();
            _response = (OkObjectResult) await handler.Run(request, NullLogger.Instance, _selectedCategory.Id , CancellationToken.None);

            _view = _response.Value as CategoryView;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.Links.AssertLink("self")
                 .ShouldBeGet()
                 .ShouldHaveSomeHref();
        }
        
        [Test]
        public void ShouldReturnCorrectCategoryData()
        {
            Assert.That(_view, Is.Not.Null, "Should contain at-least one category");
            Assert.That(_view.Id, Is.EqualTo(_selectedCategory.Id), "Category id does not match");
            Assert.That(_view.Name, Is.EqualTo(_selectedCategory.Name), "Category name does not match");
            Assert.That(_view.BookCount, Is.EqualTo(_selectedCategory.BookCategories.Count), "Category book count does not match");
        }
    }
}