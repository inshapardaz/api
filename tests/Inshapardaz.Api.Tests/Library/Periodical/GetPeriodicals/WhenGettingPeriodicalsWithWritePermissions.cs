using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.GetPeriodicals
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenGettingPeriodicalsWithWritePermissions : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<PeriodicalView> _assert;
        
        public WhenGettingPeriodicalsWithWritePermissions(Role role) 
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            PeriodicalBuilder.WithLibrary(LibraryId).WithCategories(2).WithIssues(2).Build(4);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals");

            _assert = Services.GetService<PagingAssert<PeriodicalView>>().ForResponse(_response);
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
        public void ShouldHaveCreateLink()
        {
            _assert.ShouldHaveCreateLink($"/libraries/{LibraryId}/periodicals");
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
                    .ShouldHaveUpdateLink()
                    .ShouldHaveDeleteLink()
                    .ShouldHaveIssuesLink()
                    .ShouldHaveImageLink()
                    .ShouldHaveImageUpdateLink()
                    .ShouldHaveCreateIssueLink();
            };
        }
    }
}
