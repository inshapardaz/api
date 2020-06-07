using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.GetSeries
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenGettingSeriesWithPermission : LibraryTest<Functions.Library.Series.GetSeries>
    {
        private SeriesDataBuilder _builder;
        private OkObjectResult _response;
        private PagingAssert<SeriesView> _assert;
        private readonly ClaimsPrincipal _claim;

        public WhenGettingSeriesWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            _builder = Container.GetService<SeriesDataBuilder>();
            _builder.WithLibrary(LibraryId).WithBooks(3).Build(4);

            _response = (OkObjectResult)await handler.Run(request, LibraryId, _claim, CancellationToken.None);

            _assert = new PagingAssert<SeriesView>(_response, Library);
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
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/series");
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            _assert.ShouldHaveCreateLink($"/api/library/{LibraryId}/series");
        }

        [Test]
        public void ShouldHaveEditingLinkOnSeries()
        {
            var expectedItems = _builder.Series;
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatch(item)
                      .InLibrary(LibraryId)
                      .WithBookCount(3)
                      .WithEditableLinks()
                      .ShouldHavePublicImageLink();
            }
        }
    }
}
