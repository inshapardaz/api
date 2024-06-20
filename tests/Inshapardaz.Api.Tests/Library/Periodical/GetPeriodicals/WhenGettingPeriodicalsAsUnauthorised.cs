using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.GetPeriodicals
{
    [TestFixture]
    public class WhenGettingPeriodicalsAsUnauthorised : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<PeriodicalView> _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            PeriodicalBuilder.WithLibrary(LibraryId)
                    .WithCategories(3)
                    .WithIssues(2)
                    .Build(9);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals");

            _assert = Services.GetService<PagingAssert<PeriodicalView>>()
                .ForResponse(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/periodicals");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldNotHaveNavigationLinks()
        {
            _assert.ShouldNotHaveNextLink();
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnExpectedPeriodicals()
        {
            var expectedItems = PeriodicalBuilder.Periodicals.OrderBy(a => a.Title).Take(10).ToArray();
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                Services.GetService<PeriodicalAssert>().ForView(actual).ForLibrary(LibraryId)
                    .ShouldBeSameAs(expected, 2)
                    .ShouldHaveSelfLink()
                    .ShouldNotHaveEditLinks()
                    .ShouldHaveIssuesLink()
                    .ShouldHaveImageLink()
                    .ShouldNotHaveImageUpdateLink()
                    .ShouldNotHaveCreateIssueLink();
            };
        }
    }
}
