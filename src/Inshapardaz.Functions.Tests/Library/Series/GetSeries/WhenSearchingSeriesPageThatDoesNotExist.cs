using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.GetSeries
{
    [TestFixture]
    public class WhenSearchingSeriesPageThatDoesNotExist : LibraryTest<Functions.Library.Series.GetSeries>
    {
        private SeriesDataBuilder _builder;
        private OkObjectResult _response;
        private PagingAssert<SeriesView> _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = new RequestBuilder()
                .WithQueryParameter("query", "SearchSeries")
                .WithQueryParameter("pageNumber", 100)
                .WithQueryParameter("pageSize", 10)
                .Build();

            _builder = Container.GetService<SeriesDataBuilder>();
            _builder.WithLibrary(LibraryId).WithBooks(3).WithNamePattern("SearchSeries").Build(20);

            _response = (OkObjectResult)await handler.Run(request, LibraryId, AuthenticationBuilder.Unauthorized, CancellationToken.None);

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
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/series", "query", "SearchSeries");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldNotHaveNextLink()
        {
            _assert.ShouldNotHaveNextLink();
        }

        [Test]
        public void ShouldNotHavepreviousLinks()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnNoData()
        {
            _assert.ShouldHaveNoData();
        }
    }
}
