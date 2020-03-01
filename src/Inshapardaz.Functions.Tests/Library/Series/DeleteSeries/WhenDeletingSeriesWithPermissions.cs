using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.DeleteSeries
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenDeletingSeriesWithPermissions : LibraryTest
    {
        private NoContentResult _response;

        private IEnumerable<SeriesDto> _series;
        private SeriesDto _expected;
        private SeriesDataBuilder _dataBuilder;
        private readonly ClaimsPrincipal _claim;

        public WhenDeletingSeriesWithPermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<SeriesDataBuilder>();
            _series = _dataBuilder.WithLibrary(LibraryId).Build(4);
            _expected = _series.First();

            var handler = Container.GetService<Functions.Library.Series.DeleteSeries>();
            _response = (NoContentResult)await handler.Run(request, NullLogger.Instance, LibraryId, _expected.Id, _claim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _dataBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public void ShouldHaveDeletedSeries()
        {
            var cat = DatabaseConnection.GetSeriesById(_expected.Id);
            Assert.That(cat, Is.Null, "Series should be deleted.");
        }
    }
}
