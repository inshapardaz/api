using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Ports.Database.Entities.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.DeleteCategory
{
    [TestFixture]
    public class WhenDeletingCategoryAsAnonymous : FunctionTest
    {
        private UnauthorizedResult _response;

        private IEnumerable<Category> _categories;
        private Category _selectedCategory;
        private CategoriesDataBuilder _categoriesBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _categoriesBuilder = Container.GetService<CategoriesDataBuilder>();
            _categories = _categoriesBuilder.Build(4);
            _selectedCategory = _categories.First();
            
            var handler = Container.GetService<Functions.Library.Categories.DeleteCategory>();
            _response = (UnauthorizedResult) await handler.Run(request, NullLogger.Instance, _selectedCategory.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
        }
    }
}
