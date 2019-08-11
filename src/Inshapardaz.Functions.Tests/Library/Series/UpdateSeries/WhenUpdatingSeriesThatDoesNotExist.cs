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
    public class WhenUpdatingSeriesThatDoesNotExist : FunctionTest
    {
        private CreatedResult _response;
        private SeriesDataBuilder _builder;
        private SeriesView _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<SeriesDataBuilder>();

            var handler = Container.GetService<Functions.Library.Series.UpdateSeries>();
            var faker = new Faker();
            _expected = new SeriesView { Name = new Faker().Random.String(), Description = new Faker().Random.Words(20) };
            _response = (CreatedResult) await handler.Run(_expected, NullLogger.Instance, _expected.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
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
            var returned = _response.Value as SeriesView;
            Assert.That(returned, Is.Not.Null);

            var actual = _builder.GetById(returned.Id);
            Assert.That(actual, Is.Not.Null, "Series should be created.");
        }
    }
}
