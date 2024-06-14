using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.UpdateSeries
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingSeriesWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private SeriesView _expected;
        private SeriesAssert _assert;

        public WhenUpdatingSeriesWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = SeriesBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);

            var selectedSeries = series.PickRandom();

            _expected = new SeriesView { Name = RandomData.Name };

            _response = await Client.PutObject($"/libraries/{LibraryId}/series/{selectedSeries.Id}", _expected);
            _assert = SeriesAssert.WithResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedTheSeries()
        {
            _assert.ShouldHaveSavedSeries(DatabaseConnection);
        }
    }
}
