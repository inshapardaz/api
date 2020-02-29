using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.GetCategoryById
{
    [TestFixture]
    public class WhenGettingCategoryByIdWithPermission : FunctionTest
    {
        private OkObjectResult _response;
        private CategoryView _view;
        private IEnumerable<CategoryDto> _categories;
        private CategoryDto _selectedCategory;
        private CategoriesDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<CategoriesDataBuilder>();
            _categories = _dataBuilder.Build(4);
            _selectedCategory = _categories.PickRandom();

            var handler = Container.GetService<Functions.Library.Categories.GetCategoryById>();
            _response = (OkObjectResult)await handler.Run(request, _dataBuilder.Library.Id, _selectedCategory.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);

            _view = _response.Value as CategoryView;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _dataBuilder.CleanUp();
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
        public void ShouldHaveUpdateLink()
        {
            _view.Links.AssertLink("update")
                 .ShouldBePut()
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveDeleteLink()
        {
            _view.Links.AssertLink("delete")
                 .ShouldBeDelete()
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldReturnCorrectCategoryData()
        {
            Assert.That(_view, Is.Not.Null, "Should contain at-least one category");
            Assert.That(_view.Id, Is.EqualTo(_selectedCategory.Id), "Category id does not match");
            Assert.That(_view.Name, Is.EqualTo(_selectedCategory.Name), "Category name does not match");
            var bookCount = DatabaseConnection.GetBooksByCategory(_selectedCategory.Id);
            Assert.That(_view.BookCount, Is.EqualTo(bookCount.Count()), "Category book count does not match");
        }
    }
}
