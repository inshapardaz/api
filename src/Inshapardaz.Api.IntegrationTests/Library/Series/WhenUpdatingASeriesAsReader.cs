using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View.Library;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Series
{
    [TestFixture]
    public class WhenUpdatingASeriesAsReader : IntegrationTestBase
    {
        private Domain.Entities.Library.Series _series;
        private SeriesView _updatedSeries;
        private readonly Guid _userId = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _series = SeriesDataHelper.Create("series1");

            _updatedSeries = new SeriesView {Id = _series.Id, Name = "Some New Name"};

            Response = await GetReaderClient(_userId).PutJson($"api/series/{_series.Id}", _updatedSeries);
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