using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.GetCategoryById
{
    [TestFixture]
    public class WhenGettingCategoryWithWritePermissions : LibraryTest<Functions.Library.Categories.GetCategoryById>
    {
        private OkObjectResult _response;
        private CategoriesDataBuilder _dataBuilder;
        private IEnumerable<CategoryDto> _categories;
        private CategoryDto _selectedCategory;
        private CategoryAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            _dataBuilder = Container.GetService<CategoriesDataBuilder>();
            _categories = _dataBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);
            _selectedCategory = _categories.First();

            _response = (OkObjectResult)await handler.Run(request, LibraryId, _selectedCategory.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);

            _assert = CategoryAssert.FromResponse(_response).InLibrary(LibraryId);
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
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldNotEditLinks()
        {
            _assert.ShouldHaveUpdateLink();
            _assert.ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveBooksLink()
        {
            _assert.ShouldHaveBooksLink();
        }

        [Test]
        public void ShouldReturnCorrectCategoryData()
        {
            _assert.ShouldBeSameAs(_selectedCategory);
        }
    }
}
