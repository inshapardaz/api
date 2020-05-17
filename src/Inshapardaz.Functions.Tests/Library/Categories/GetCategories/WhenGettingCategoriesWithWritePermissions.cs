using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.GetCategories
{
    [TestFixture]
    public class WhenGettingCategoriesWithWritePermissions : LibraryTest<Functions.Library.Categories.GetCategories>
    {
        private OkObjectResult _response;
        private CategoriesDataBuilder _dataBuilder;
        private IEnumerable<CategoryDto> _categories;
        private ListView<CategoryView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            _dataBuilder = Container.GetService<CategoriesDataBuilder>();
            _categories = _dataBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);

            _response = (OkObjectResult)await handler.Run(request, LibraryId, AuthenticationBuilder.AdminClaim, CancellationToken.None);

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
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.SelfLink()
                .ShouldBeGet()
                .EndingWith($"/api/library/{LibraryId}/categories");
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            _view.CreateLink()
                .ShouldBePost()
                .EndingWith($"/api/library/{LibraryId}/categories");
        }

        [Test]
        public void ShouldHaveSomeCategories()
        {
            foreach (var item in _categories)
            {
                var actual = _view.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatch(item)
                      .InLibrary(LibraryId)
                      .WithBookCount(3)
                      .ShouldHaveUpdateLink()
                      .ShouldHaveDeleteLink();
            }
        }
    }
}
