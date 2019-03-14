using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Series
{
    [TestFixture]
    public class WhenGettingSeriesByIdAsAdministrator : IntegrationTestBase
    {
        private SeriesView _view;
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Library.Series _series;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _series = SeriesDataHelper.Create(TestDataProvider.RandomString);

            Response = await GetAdminClient(_userId).GetAsync($"/api/series/{_series.Id}");
            _view = JsonConvert.DeserializeObject<SeriesView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            SeriesDataHelper.Delete(_series.Id);
        }

        [Test]
        public void ShouldReturnOk()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldReturnSeriesWithCorrectId()
        {
            _view.Id.ShouldBe(_view.Id);
        }

        [Test]
        public void ShouldReturnSeriesWithCorrectName()
        {
            _view.Name.ShouldBe(_series.Name);
        }

        [Test]
        public void ShouldReturnSelfLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Self);
        }

        [Test]
        public void ShouldReturnUpdateLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Update);
        }

        [Test]
        public void ShouldReturnDeleteLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Delete);
        }
    }
}