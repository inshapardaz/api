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
    public class WhenAddingSeriesAsReader : IntegrationTestBase
    {
        private SeriesView _seriesView;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _seriesView = new SeriesView
            {
                Name = "Some Name"
            };

            Response = await GetReaderClient(Guid.NewGuid()).PostJson($"/api/series", _seriesView);
        }
        
        [Test]
        public void ShouldReturnForbidden()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        }
    }
}