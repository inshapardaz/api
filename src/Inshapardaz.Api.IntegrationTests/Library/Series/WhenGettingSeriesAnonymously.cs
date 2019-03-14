using System.Collections.Generic;
using System.Linq;
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
    public class WhenGettingSeriesAnonymously : IntegrationTestBase
    {
        private readonly List<Domain.Entities.Library.Series> _series = new List<Domain.Entities.Library.Series>();
        private ListView<SeriesView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _series.Add(SeriesDataHelper.Create(TestDataProvider.RandomString));
            _series.Add(SeriesDataHelper.Create(TestDataProvider.RandomString));
            _series.Add(SeriesDataHelper.Create(TestDataProvider.RandomString));
            _series.Add(SeriesDataHelper.Create(TestDataProvider.RandomString));

             Response = await GetClient().GetAsync("/api/series/");
            _view = JsonConvert.DeserializeObject<ListView<SeriesView>>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            foreach (var series in _series)
            {
                SeriesDataHelper.Delete(series.Id);
            }
        }

        [Test]
        public void ShouldReturnOk()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldContainAllCategories()
        {
            _view.Items.Count().ShouldBeGreaterThanOrEqualTo(_series.Count);
            foreach (var series in _series)
            {
                _view.Items.ShouldContain(v => v.Name == series.Name);
            }
        }

        [Test]
        public void ShouldReturnSelfLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Self);
        }

        [Test]
        public void ShouldNotReturnCreateLink()
        {
            _view.Links.ShouldNotContain(link => link.Rel == RelTypes.Create);
        }
    }
}