using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View.Library;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Series
{
    [TestFixture, Ignore("Role based security not implemented")]
    public class WhenUpdatingASeries : IntegrationTestBase
    {
        private Domain.Entities.Library.Series _view;
        private Domain.Entities.Library.Series _series;
        private readonly Guid _userId = Guid.NewGuid();
        private SeriesView _updatedSeries;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _series = SeriesDataHelper.Create(TestDataProvider.RandomString);

            _updatedSeries = new SeriesView {Id = _series.Id, Name = TestDataProvider.RandomString };

            Response = await GetAdminClient(_userId).PutJson($"api/series/{_series.Id}", _updatedSeries);

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
        public void ShouldHaveUpdateTheTranslation()
        {
            _view.Name.ShouldBe(_updatedSeries.Name);
        }
    }
}