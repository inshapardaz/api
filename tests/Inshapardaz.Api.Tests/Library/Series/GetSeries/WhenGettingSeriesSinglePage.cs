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
    public class WhenGettingSeriesSinglePage : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<SeriesView> _assert;

        public WhenGettingSeriesSinglePage()
            :base(Domain.Models.Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            SeriesBuilder.WithLibrary(LibraryId).WithBooks(3).WithoutImage().Build(4);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/series");
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
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnExpectedSeries()
        {
            var expectedItems = SeriesBuilder.Series;
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                Services.GetService<SeriesAssert>().ForView(actual)
                    .InLibrary(LibraryId)
                    .ShouldBeSameAs(item)
                    .WithBookCount(3)
                    .WithReadOnlyLinks()
                    .ShouldNotHaveImageLink();
            }
        }
    }
}
