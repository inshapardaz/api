using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.DeleteCategory
{
    [TestFixture]
    public class WhenDeletingCategoryAsAnonymous : LibraryTest<Functions.Library.Categories.DeleteCategory>
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
            _categories = _dataBuilder.WithLibrary(LibraryId).Build(4);
            _selectedCategory = _categories.PickRandom();

            _response = (UnauthorizedResult)await handler.Run(request, LibraryId, _selectedCategory.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _dataBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
