using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View.Library;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Series
{
    [TestFixture]
    public class WhenDeletingASeriesAsAnonymous : IntegrationTestBase
    {
        private Domain.Entities.Library.Series _series;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _series = SeriesDataHelper.Create("series1");

            Response = await GetClient().DeleteAsync($"api/series/{_series.Id}");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            SeriesDataHelper.Delete(_series.Id);
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}