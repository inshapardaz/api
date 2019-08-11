using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.GetCategories
{
    [TestFixture]
    public class WhenGettingCategories : FunctionTest
    {
        OkObjectResult _response;
        ListView<CategoryView> _view;
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var categoriesBuilder = Container.GetService<CategoriesDataBuilder>();
            categoriesBuilder.WithBooks(3).Build(4);
            
            var handler = Container.GetService<Functions.Library.Categories.GetCategories>();
            _response = (OkObjectResult) await handler.Run(request, NullLogger.Instance, AuthenticationBuilder.Unauthorized, CancellationToken.None);

            _view = _response.Value as ListView<CategoryView>;
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
        public void ShouldHaveSomeCategories()
        {
            Assert.IsNotEmpty(_view.Items, "Should return some categories.");
            Assert.That(_view.Items.Count(), Is.EqualTo(4), "Should return all categories");
        }

        [Test]
        public void ShouldHaveCorrectCategoryData()
        {
            var firstCategory = _view.Items.FirstOrDefault();
            Assert.That(firstCategory, Is.Not.Null, "Should contain at-least one category");
            Assert.That(firstCategory.Name, Is.Not.Empty, "Category name should have a value");
            Assert.That(firstCategory.BookCount, Is.GreaterThan(0), "Category name should have a value");
        }
    }
}
