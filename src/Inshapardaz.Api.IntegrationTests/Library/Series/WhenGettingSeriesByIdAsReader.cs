using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Series
{
    [TestFixture]
    public class WhenGettingSeriesByIdAsReader : IntegrationTestBase
    {
        private SeriesView _view;
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Library.Series _series;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _series = SeriesDataHelper.Create("Series2323");

            Response = await GetReaderClient(_userId).GetAsync($"/api/series/{_series.Id}");
            _view = JsonConvert.DeserializeObject<SeriesView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(_series.Id);
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
        public void ShouldNotReturnUpdateLink()
        {
            _view.Links.ShouldNotContain(link => link.Rel == RelTypes.Update);
        }

        [Test]
        public void ShouldNotReturnDeleteLink()
        {
            _view.Links.ShouldNotContain(link => link.Rel == RelTypes.Delete);
        }
    }
}