using FluentAssertions;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;
using System.Collections.Generic;
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

            _assert = new PagingAssert<PeriodicalView>(_response);
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
                actual.ShouldMatch(expected, 2, DatabaseConnection, LibraryId)
                            .InLibrary(LibraryId)
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
