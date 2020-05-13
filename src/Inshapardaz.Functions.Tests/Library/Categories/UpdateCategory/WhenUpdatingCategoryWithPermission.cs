using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.UpdateCategory
{
    [TestFixture]
    public class WhenUpdatingCategoryWithPermission : LibraryTest<Functions.Library.Categories.UpdateCategory>
    {
        private OkObjectResult _response;
        private CategoriesDataBuilder _categoriesBuilder;
        private IEnumerable<CategoryDto> _categories;
        private CategoryDto _selectedCategory;
        private CategoryView _expectedCategory;
        private CategoryAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categoriesBuilder = Container.GetService<CategoriesDataBuilder>();

            _categories = _categoriesBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);

            _selectedCategory = _categories.First();

            _expectedCategory = new CategoryView { Name = new Faker().Name.FullName() };

            _response = (OkObjectResult)await handler.Run(_expectedCategory, LibraryId, _selectedCategory.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
            _assert = CategoryAssert.FromResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _categoriesBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedTheCategory()
        {
            _assert.ShouldBeSameAs(_expectedCategory);
        }

        [Test]
        public void ShouldHaveUpdatedCategory()
        {
            _assert.ShouldHaveUpdatedCategory(DatabaseConnection);
        }
    }
}
