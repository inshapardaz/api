using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    [TestFixture(AuthenticationLevel.Reader)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenDeletingCategoryWithoutPermissions : FunctionTest
    {
        private ForbidResult _response;

        private IEnumerable<CategoryDto> _categories;
        private CategoryDto _selectedCategory;
        private CategoriesDataBuilder _dataBuilder;
        private readonly ClaimsPrincipal _claim;

        public WhenDeletingCategoryWithoutPermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<CategoriesDataBuilder>();
            _categories = _dataBuilder.Build(4);
            _selectedCategory = _categories.First();

            var handler = Container.GetService<Functions.Library.Categories.DeleteCategory>();
            _response = (ForbidResult)await handler.Run(request, _dataBuilder.Library.Id, _selectedCategory.Id, _claim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _dataBuilder.CleanUp();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
