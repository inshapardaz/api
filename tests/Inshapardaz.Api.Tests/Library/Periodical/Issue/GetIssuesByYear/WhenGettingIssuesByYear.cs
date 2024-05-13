using FluentAssertions;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.GetIssuesByYear
{
    [TestFixture]
    public class WhenGettingIssuesByYear
        : TestBase
    {
        private HttpResponseMessage _response;
        private int _periodicalId;
        private IEnumerable<IssueDto> _expected;

        private IssueYearlyView _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodical = PeriodicalBuilder.WithLibrary(LibraryId).Build();
            _periodicalId = periodical.Id;
            _expected = IssueBuilder.WithLibrary(LibraryId)
                                    .WithPages(10)
                                    .WithPeriodical(_periodicalId)
                                    .Build(40);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_periodicalId}/issues/years");

            _view = _response.GetContent<IssueYearlyView>().Result;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveCorrectObjectRetured()
        {
            var expectedSummary = _expected.GroupBy(x => x.IssueDate.Year)
                .ToDictionary(x => x.Key, x => x.Count())
                .Select(y => new IssueYearView
                {
                    Year = y.Key,
                    Count = y.Value,
                    Links = new List<Views.LinkView>
                    {
                        new Views.LinkView
                        {
                            Method = "GET",
                            Rel = "self",
                            Href = $"http://localhost/libraries/{LibraryId}/periodicals/{_periodicalId}/issues?year={y.Key}"
                        }
                    }
                });

            _view.Data.Should().BeEquivalentTo(expectedSummary);
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.Links.Self().Should().Be($"http://localhost/libraries/{LibraryId}/periodicals/{_periodicalId}/issues/years");
        }
    }
}
