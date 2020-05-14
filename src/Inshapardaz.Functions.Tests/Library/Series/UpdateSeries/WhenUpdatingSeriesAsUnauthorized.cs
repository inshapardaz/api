using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.UpdateSeries
{
    [TestFixture]
    public class WhenUpdatingSeriesAsUnauthorized : LibraryTest<Functions.Library.Series.UpdateSeries>
    {
        private UnauthorizedResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var faker = new Faker();
            var series = new SeriesView { Id = faker.Random.Number(), Name = faker.Random.String() };
            _response = (UnauthorizedResult)await handler.Run(series, LibraryId, series.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
