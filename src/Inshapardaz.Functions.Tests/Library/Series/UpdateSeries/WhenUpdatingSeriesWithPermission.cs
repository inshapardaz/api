using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.UpdateSeries
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenUpdatingSeriesWithPermission : LibraryTest<Functions.Library.Series.UpdateSeries>
    {
        private OkObjectResult _response;
        private SeriesDataBuilder _dataBuilder;
        private SeriesView _expected;
        private readonly ClaimsPrincipal _claim;
        private SeriesAssert _assert;

        public WhenUpdatingSeriesWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<SeriesDataBuilder>();

            var series = _dataBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);

            var selectedSeries = series.First();

            _expected = new SeriesView { Name = new Faker().Name.FullName() };

            _response = (OkObjectResult)await handler.Run(_expected, LibraryId, selectedSeries.Id, _claim, CancellationToken.None);
            _assert = SeriesAssert.WithResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _dataBuilder.CleanUp();
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
