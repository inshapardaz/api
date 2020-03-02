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

namespace Inshapardaz.Functions.Tests.Library.Series.GetSeries
{
    [TestFixture]
    public class WhenGettingSeriesAsAnonymous : LibraryTest<Functions.Library.Series.GetSeries>
    {
        private OkObjectResult _response;
        private ListView<SeriesView> _view;
        private SeriesDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            _dataBuilder = Container.GetService<SeriesDataBuilder>();
            _dataBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);

            _response = (OkObjectResult)await handler.Run(request, NullLogger.Instance, LibraryId, AuthenticationBuilder.Unauthorized, CancellationToken.None);

            _view = _response.Value as ListView<SeriesView>;
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
        public void ShouldHaveSomeSeries()
        {
            Assert.IsNotEmpty(_view.Items, "Should return some series.");
            Assert.That(_view.Items.Count(), Is.EqualTo(4), "Should return all series");
        }

        [Test]
        public void ShouldHaveCorrectSeriesData()
        {
            var firstSeries = _view.Items.FirstOrDefault();
            Assert.That(firstSeries, Is.Not.Null, "Should contain at-least one series");
            Assert.That(firstSeries.Name, Is.Not.Empty, "Series name should have a value");
            Assert.That(firstSeries.Description, Is.Not.Empty, "Series description should have a value");
            Assert.That(firstSeries.BookCount, Is.GreaterThan(0), "Series should have some books");
        }
    }
}
