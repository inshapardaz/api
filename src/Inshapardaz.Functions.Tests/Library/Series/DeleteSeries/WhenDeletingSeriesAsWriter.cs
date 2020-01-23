using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.DeleteSeries
{
    [TestFixture]
    public class WhenDeletingSeriesAsWriter : FunctionTest
    {
        private NoContentResult _response;

        private IEnumerable<Ports.Database.Entities.Library.Series> _series;
        private Ports.Database.Entities.Library.Series _expected;
        private SeriesDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<SeriesDataBuilder>();
            _series = _dataBuilder.Build(4);
            _expected = _series.First();
            
            var handler = Container.GetService<Functions.Library.Series.DeleteSeries>();
            _response = (NoContentResult) await handler.Run(request, NullLogger.Instance, _expected.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
            Assert.That(_response.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public void ShouldHaveDeletedSeries()
        {
            var cat = _dataBuilder.GetById(_expected.Id);
            Assert.That(cat, Is.Null, "Series should be deleted.");

        }
    }
}
