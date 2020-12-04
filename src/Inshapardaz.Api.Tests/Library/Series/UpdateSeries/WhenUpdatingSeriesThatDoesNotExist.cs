using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.UpdateSeries
{
    [TestFixture]
    public class WhenUpdatingSeriesThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private SeriesView _series;
        private SeriesAssert _assert;

        public WhenUpdatingSeriesThatDoesNotExist()
            : base(Role.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _series = new SeriesView { Name = Random.Name };

            _response = await Client.PutObject($"/libraries/{LibraryId}/series/{-Random.Number}", _series);
            _assert = SeriesAssert.WithResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _assert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldHaveCreatedTheSeries()
        {
            _assert.ShouldHaveSavedSeries(DatabaseConnection);
        }
    }
}