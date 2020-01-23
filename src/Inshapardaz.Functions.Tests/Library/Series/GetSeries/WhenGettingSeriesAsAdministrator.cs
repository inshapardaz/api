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

namespace Inshapardaz.Functions.Tests.Library.Series.GetSeries
{
    [TestFixture]
    public class WhenGettingSeriesAsAdministrator : FunctionTest
    {
        private OkObjectResult _response;
        private ListView<SeriesView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var seriesBuilder = Container.GetService<SeriesDataBuilder>();
            seriesBuilder.WithBooks(3).Build(4);

            var handler = Container.GetService<Functions.Library.Series.GetSeries>();
            _response = (OkObjectResult)await handler.Run(request, NullLogger.Instance, AuthenticationBuilder.WriterClaim, CancellationToken.None);

            _view = _response.Value as ListView<SeriesView>;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
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
