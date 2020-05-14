using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.GetSeriesById
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenGettingSeriesWithPermission : LibraryTest<Functions.Library.Series.GetSeriesById>
    {
        private SeriesDataBuilder _builder;
        private OkObjectResult _response;
        private SeriesDto _expected;
        private readonly ClaimsPrincipal _claim;
        private SeriesAssert _assert;

        public WhenGettingSeriesWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _builder = Container.GetService<SeriesDataBuilder>();
            var series = _builder.WithLibrary(LibraryId).Build(4);
            _expected = series.First();

            _response = (OkObjectResult)await handler.Run(request, LibraryId, _expected.Id, _claim, CancellationToken.None);

            _assert = SeriesAssert.WithResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldHaveBooksLink()
        {
            _assert.ShouldHaveBooksLink();
        }

        [Test]
        public void ShouldHaveUpdateLink()
        {
            _assert.ShouldHaveUpdateLink();
        }

        [Test]
        public void ShouldHaveDeleteLink()
        {
            _assert.ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveImageUploadLink()
        {
            _assert.ShouldHaveImageUploadLink();
        }

        [Test]
        public void ShouldReturnCorrectSeriesData()
        {
            _assert.ShouldHaveCorrectSeriesRetunred(_expected, DatabaseConnection);
        }
    }
}
