using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.GetSeries
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenGettingSeriesWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<SeriesView> _assert;

        public WhenGettingSeriesWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            SeriesBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);

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
        public void ShouldHaveCreateLink()
        {
            _assert.ShouldHaveCreateLink($"/libraries/{LibraryId}/series");
        }

        [Test]
        public void ShouldHaveEditingLinkOnSeries()
        {
            var expectedItems = SeriesBuilder.Series;
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                Services.GetService<SeriesAssert>().ForView(actual)
                    .InLibrary(LibraryId)
                    .ShouldBeSameAs(item)
                    .WithBookCount(3)
                    .WithEditableLinks()
                    .ShouldHavePublicImageLink();
            }
        }
    }
}
