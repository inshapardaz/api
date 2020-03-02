using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Inshapardaz.Functions.Tests.DataHelpers;
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
        private SeriesView _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expected = new Fixture().Build<SeriesView>().Without(s => s.Links).Without(s => s.BookCount).Create();
            var request = new RequestBuilder()
                                            .WithJsonBody(_expected)
                                            .Build();
            _response = (CreatedResult)await handler.Run(request, LibraryId, _expected.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            Assert.That(_response.Location, Is.Not.Empty);
        }

        [Test]
        public void ShouldHaveCreatedTheSeries()
        {
            var returned = _response.Value as SeriesView;
            Assert.That(returned, Is.Not.Null);

            var actual = DatabaseConnection.GetSeriesById(returned.Id);
            Assert.That(actual, Is.Not.Null, "Series should be created.");
        }
    }
}
