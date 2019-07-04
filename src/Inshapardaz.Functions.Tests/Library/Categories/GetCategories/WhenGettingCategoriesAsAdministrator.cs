using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.GetCategories
{
    [TestFixture]
    public class WhenGettingCategoriesAsAdministrator : FunctionTest
    {
        OkObjectResult _response;
        ListView<CategoryView> _view;
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            AuthenticateAsAdmin();
            var categoriesBuilder = Container.GetService<CategoriesDataBuilder>();
            categoriesBuilder.WithCategories(4).Build();
            
            var handler = Container.GetService<Functions.Library.Categories.GetCategories>();
            _response = (OkObjectResult) await handler.Run(request, NullLogger.Instance, CancellationToken.None);

            _view = _response.Value as ListView<CategoryView>;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
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