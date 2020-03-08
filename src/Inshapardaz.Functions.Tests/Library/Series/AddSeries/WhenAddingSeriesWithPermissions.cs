using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.AddSeries
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenAddingSeriesWithPermissions : LibraryTest<Functions.Library.Series.AddSeries>
    {
        private CreatedResult _response;
        private LibraryDataBuilder _builder;
        private readonly ClaimsPrincipal _claim;

        public WhenAddingSeriesWithPermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = new Fixture().Build<SeriesView>().Without(s => s.Links).Without(s => s.BookCount).Create();

            _response = (CreatedResult)await handler.Run(series, LibraryId, _claim, CancellationToken.None);
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
            var series = _response.Value as SeriesView;
            Assert.That(series, Is.Not.Null);

            var cat = DatabaseConnection.GetSeriesById(series.Id);
            Assert.That(cat, Is.Not.Null, "Series should be created.");
        }
    }
}
