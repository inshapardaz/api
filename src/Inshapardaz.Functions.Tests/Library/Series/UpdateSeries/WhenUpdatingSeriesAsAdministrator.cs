using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.UpdateSeries
{
    [TestFixture]
    public class WhenUpdatingSeriesAsAdministrator : FunctionTest
    {
        OkObjectResult _response;
        private SeriesDataBuilder _dataBuilder;
        private IEnumerable<Ports.Database.Entities.Library.Series> _series;
        private SeriesView _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<SeriesDataBuilder>();

            var handler = Container.GetService<Functions.Library.Series.UpdateSeries>();
            _series = _dataBuilder.WithBooks(3).Build(4);

            var selectedSeries = _series.First();

            _expected = new SeriesView { Name = new Faker().Random.String(), Description = new Faker().Random.Words(20) };
            _response = (OkObjectResult) await handler.Run(_expected, NullLogger.Instance, selectedSeries.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int) HttpStatusCode.OK));
        }

        [Test]
        public void ShouldHaveUpdatedTheSeries()
        {
            var returned = _response.Value as SeriesView;
            Assert.That(returned, Is.Not.Null);

            var actual = _dataBuilder.GetById(returned.Id);
            Assert.That(actual.Name, Is.EqualTo(_expected.Name), "Series should have expected name.");
            Assert.That(actual.Description, Is.EqualTo(_expected.Description), "Series should have expected description.");
        }
    }
}
