using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.GetSeries
{
    [TestFixture]
    public class WhenSearchingSeriesFirstPage : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<SeriesView> _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SeriesBuilder.WithLibrary(LibraryId).WithBooks(3).WithNamePattern("SearchSeries").Build(20);

            _response = await Client.GetAsync($"/library/{LibraryId}/series?query=SearchSeries&pageNumber=1&pageSize=10");
            _assert = new PagingAssert<SeriesView>(_response, Library);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
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
            _assert.ShouldHaveSelfLink($"/library/{LibraryId}/series", "query", "SearchSeries");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/library/{LibraryId}/series", 2, 10, "query", "SearchSeries");
        }

        [Test]
        public void ShouldNotHavepreviousLinks()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnExpectedSeries()
        {
            var expectedItems = SeriesBuilder.Series.Where(a => a.Name.Contains("SearchSeries")).OrderBy(a => a.Name).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatch(item)
                      .InLibrary(LibraryId)
                      .WithBookCount(3)
                      .WithReadOnlyLinks()
                      .ShouldHavePublicImageLink();
            }
        }
    }
}
