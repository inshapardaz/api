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
    public class WhenGettingCategoriesWithPermission : FunctionTest
    {
        private OkObjectResult _response;
        private ListView<CategoryView> _view;
        private CategoriesDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<CategoriesDataBuilder>();
            _dataBuilder.Build(4);

            var handler = Container.GetService<Functions.Library.Categories.GetCategories>();
            _response = (OkObjectResult)await handler.Run(request, _dataBuilder.Library.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);

            _view = _response.Value as ListView<CategoryView>;
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
        public void ShouldHaveCreateLink()
        {
            _view.Links.AssertLink("create")
                 .ShouldBePost()
                 .ShouldHaveSomeHref();
        }
    }
}
