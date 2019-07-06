using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.GetSeriesById
{
    [TestFixture]
    public class WhenGettingSeriesByIdForSeriesThatDoesNotExist : FunctionTest
    {
        NotFoundResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            
            var handler = Container.GetService<Functions.Library.Series.GetSeriesById>();
            _response = (NotFoundResult) await handler.Run(request, NullLogger.Instance, new Faker().Random.Int(), AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
            Assert.That(_response.StatusCode, Is.EqualTo((int) HttpStatusCode.NotFound));
        }
    }
}
