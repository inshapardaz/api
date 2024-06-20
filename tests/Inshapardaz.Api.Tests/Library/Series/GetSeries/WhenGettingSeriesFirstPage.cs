using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.GetSeries
{
    [TestFixture]
    public class WhenGettingSeriesFirstPage : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<SeriesView> _assert;

        public WhenGettingSeriesFirstPage()
            :base(Domain.Models.Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            SeriesBuilder.WithLibrary(LibraryId).WithBooks(3).Build(20);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/series?pageNumber=1&pageSize=10");
            _assert = Services.GetService<PagingAssert<SeriesView>>().ForResponse(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/series");
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/series", 2, 10);
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldHaveCorrectSeriesData()
        {
            var expectedItems = SeriesBuilder.Series.OrderBy(a => a.Name).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                Services.GetService<SeriesAssert>().ForView(actual)
                    .InLibrary(LibraryId)
                    .ShouldBeSameAs(item)
                    .WithBookCount(3)
                    .WithReadOnlyLinks()
                    .ShouldHavePublicImageLink();
            }
        }
    }
}
