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
    public class WhenDeletingSeriesAsReader : FunctionTest
    {
        ForbidResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            var builder = Container.GetService<SeriesDataBuilder>();
            var series = builder.WithSeries(4).Build();
            var expected = series.First();
            
            var handler = Container.GetService<Functions.Library.Series.DeleteSeries>();
            _response = (ForbidResult) await handler.Run(request, NullLogger.Instance, expected.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
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
        }
    }
}
