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
    public class WhenAddingSeriesAsAnonymous : IntegrationTestBase
    {
        private SeriesView _seriesView;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _seriesView = new SeriesView
            {
                Name = TestDataProvider.RandomString
            };

            Response = await GetClient().PostJson("/api/series", _seriesView);
        }
        
        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}