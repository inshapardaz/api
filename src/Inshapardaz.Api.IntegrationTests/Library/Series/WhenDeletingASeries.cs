using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Series
{
    [TestFixture, Ignore("Role based security not implemented")]
    public class WhenDeletingASeries : IntegrationTestBase
    {
        private Domain.Entities.Library.Series _view;
        private Domain.Entities.Library.Series _series;
        private readonly Guid _userId = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _series = SeriesDataHelper.Create("series1");

            Response = await GetAdminClient(_userId).DeleteAsync($"api/series/{_series.Id}");

            _view = SeriesDataHelper.Get(_series.Id);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            SeriesDataHelper.Delete(_series.Id);
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Test]
        public void ShouldHaveDeletedTheTranslation()
        {
            _view.ShouldBeNull();
        }
    }
}