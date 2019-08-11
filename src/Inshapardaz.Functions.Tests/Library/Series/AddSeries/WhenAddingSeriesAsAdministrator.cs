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

namespace Inshapardaz.Functions.Tests.Library.Series.AddSeries
{
    [TestFixture]
    public class WhenAddingSeriesAsAdministrator : FunctionTest
    {
        private CreatedResult _response;
        private SeriesDataBuilder _builder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<SeriesDataBuilder>();
            
            var handler = Container.GetService<Functions.Library.Series.AddSeries>();
            var request = new SeriesView { Name = new Faker().Random.String(), Description = new Faker().Random.Words(30) };
            _response = (CreatedResult) await handler.Run(request, NullLogger.Instance, AuthenticationBuilder.AdminClaim, CancellationToken.None);
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
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            Assert.That(_response.Location, Is.Not.Empty);
        }

        [Test]
        public void ShouldHaveCreatedTheSeries()
        {
            var series = _response.Value as SeriesView;
            Assert.That(series, Is.Not.Null);

            var cat = _builder.GetById(series.Id);
            Assert.That(cat, Is.Not.Null, "Series should be created.");
        }
    }
}
