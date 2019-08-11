using System.Linq;
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

namespace Inshapardaz.Functions.Tests.Library.Series.GetSeriesById
{
    [TestFixture]
    public class WhenGettingSeriesByIdAsReader : FunctionTest
    {
        OkObjectResult _response;
        SeriesView _view;
        private Ports.Database.Entities.Library.Series _selectedSeries;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var categoriesBuilder = Container.GetService<SeriesDataBuilder>();
            var series = categoriesBuilder.WithBooks(3).Build(4);
            _selectedSeries = series.First();
            
            var handler = Container.GetService<Functions.Library.Series.GetSeriesById>();
            _response = (OkObjectResult) await handler.Run(request, NullLogger.Instance, _selectedSeries.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);

            _view = _response.Value as SeriesView;
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
        public void ShouldReturnCorrectCategoryData()
        {
            Assert.That(_view, Is.Not.Null, "Should contain at-least one series");
            Assert.That(_view.Id, Is.EqualTo(_selectedSeries.Id), "Series id does not match");
            Assert.That(_view.Name, Is.EqualTo(_selectedSeries.Name), "Series name does not match");
            Assert.That(_view.Description, Is.EqualTo(_selectedSeries.Description), "Series description does not match");
            Assert.That(_view.BookCount, Is.EqualTo(_selectedSeries.Books.Count), "Series book count does not match");

            _view.Links.AssertLink(RelTypes.Self)
                 .ShouldBeGet()
                 .ShouldHaveSomeHref();

            _view.Links.AssertLinkNotPresent(RelTypes.Update);
            _view.Links.AssertLinkNotPresent(RelTypes.Delete);
        }
    }
}
