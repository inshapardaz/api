using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.DeleteCategory
{
    [TestFixture]
    public class WhenDeletingCategoryAsAnonymous : FunctionTest
    {
        private UnauthorizedResult _response;

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

            var handler = Container.GetService<Functions.Library.Categories.DeleteCategory>();
            _response = (UnauthorizedResult)await handler.Run(request, _dataBuilder.Library.Id, _selectedCategory.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _dataBuilder.CleanUp();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
        }
    }
}
