using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.GetCategories
{
    [TestFixture]
    public class WhenGettingCategoriesAsUnauthorises : LibraryTest
    {
        private OkObjectResult _response;
        private ListView<CategoryView> _view;
        private CategoriesDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            _dataBuilder = Container.GetService<CategoriesDataBuilder>();
            _dataBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);

            var handler = Container.GetService<Functions.Library.Categories.GetCategories>();
            _response = (OkObjectResult)await handler.Run(request, LibraryId, AuthenticationBuilder.Unauthorized, CancellationToken.None);

            _view = _response.Value as ListView<CategoryView>;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _dataBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
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
            Assert.That(firstCategory.BookCount, Is.GreaterThan(0), "Category should have some books");
        }
    }
}
