using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Series
{
    [TestFixture]
    public class WhenDeletingASeriesAsReader : IntegrationTestBase
    {
        private Domain.Entities.Library.Series _series;
        private readonly Guid _userId = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _series = SeriesDataHelper.Create("series1");

            Response = await GetReaderClient(_userId).DeleteAsync($"api/series/{_series.Id}");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            SeriesDataHelper.Delete(_series.Id);
        }

        [Test]
        public void ShouldReturnForbidden()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        }
    }
}