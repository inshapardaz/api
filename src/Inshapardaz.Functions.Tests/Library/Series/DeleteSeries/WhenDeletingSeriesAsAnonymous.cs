using System.Net;
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
    public class WhenDeletingSeriesAsAnonymous : LibraryTest
    {
        private UnauthorizedResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            var builder = Container.GetService<SeriesDataBuilder>();
            var series = builder.WithLibrary(LibraryId).Build();

            var handler = Container.GetService<Functions.Library.Series.DeleteSeries>();
            _response = (UnauthorizedResult)await handler.Run(request, NullLogger.Instance, LibraryId, series.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
        }
    }
}
