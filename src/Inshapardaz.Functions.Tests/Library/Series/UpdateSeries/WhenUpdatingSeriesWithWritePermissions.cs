using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.UpdateSeries
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenUpdatingSeriesWithWritePermissions : FunctionTest
    {
        private OkObjectResult _response;
        private SeriesDataBuilder _dataBuilder;
        private IEnumerable<SeriesDto> _series;
        private SeriesView _expected;
        private readonly ClaimsPrincipal _claim;

        public WhenUpdatingSeriesWithWritePermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<SeriesDataBuilder>();

            var handler = Container.GetService<Functions.Library.Series.UpdateSeries>();
            _series = _dataBuilder.WithBooks(3).Build(4);

            var selectedSeries = _series.First();

            _expected = new SeriesView { Name = new Faker().Name.LastName(), Description = new Faker().Random.Words(20) };
            var request = new RequestBuilder()
                                            .WithJsonBody(_expected)
                                            .Build();
            _response = (OkObjectResult)await handler.Run(request, _dataBuilder.Library.Id, selectedSeries.Id, _claim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _dataBuilder.CleanUp();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public void ShouldHaveUpdatedTheSeries()
        {
            var returned = _response.Value as SeriesView;
            Assert.That(returned, Is.Not.Null);

            var actual = DatabaseConnection.GetSeriesById(returned.Id);

            Assert.That(actual.Name, Is.EqualTo(_expected.Name), "Series should have expected name.");
            Assert.That(actual.Description, Is.EqualTo(_expected.Description), "Series should have expected description.");
        }
    }
}
