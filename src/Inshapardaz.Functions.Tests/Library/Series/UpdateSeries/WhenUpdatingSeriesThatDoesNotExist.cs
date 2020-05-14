using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.UpdateSeries
{
    [TestFixture]
    public class WhenUpdatingSeriesThatDoesNotExist : LibraryTest<Functions.Library.Series.UpdateSeries>
    {
        private CreatedResult _response;
        private SeriesView _series;
        private SeriesAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _series = new SeriesView { Name = new Faker().Random.String() };

            _response = (CreatedResult)await handler.Run(_series, LibraryId, _series.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
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
