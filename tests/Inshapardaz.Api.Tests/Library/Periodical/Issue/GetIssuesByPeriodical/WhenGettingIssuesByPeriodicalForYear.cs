using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.GetIssuesByPeriodical
{
    public class WhenGettingIssuesByPeriodicalForYear
        : TestBase
    {
        private HttpResponseMessage _response;
        private IEnumerable<IssueDto> _issues;
        private PeriodicalDto _periodical;
        private PagingAssert<IssueView> _assert;

        public WhenGettingIssuesByPeriodicalForYear()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _periodical = PeriodicalBuilder.WithLibrary(LibraryId).Build();
            _issues = IssueBuilder.WithLibrary(LibraryId).WithPeriodical(_periodical.Id).WithPublishYear(1999).WithContent().Build(15);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_periodical.Id}/issues?year=1999&pageNumber=2&pageSize=5");
            _assert = Services.GetService<PagingAssert<IssueView>>().ForResponse(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/periodicals/{_periodical.Id}/issues", new KeyValuePair<string, string>("year", "1999"));
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/periodicals/{_periodical.Id}/issues", 3, 5, new KeyValuePair<string, string>("year", "1999"));
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/periodicals/{_periodical.Id}/issues", 1, 5, new KeyValuePair<string, string>("year", "1999"));
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            _assert.ShouldHaveCreateLink($"/libraries/{LibraryId}/periodicals/{_periodical.Id}/issues");
        }

        [Test]
        public void ShouldHaveCorrectIssuessData()
        {
            var expectedItems = _issues.Where(x => x.IssueDate.Year == 1999).OrderBy(a => a.IssueDate).Skip(5).Take(5).ToArray();
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                var pages = IssueBuilder.GetPages(expected.Id);
                var articles = IssueBuilder.GetArticles(expected.Id);
                var assert = Services.GetService<IssueAssert>().ForView(actual).ForLibrary(LibraryId)
                    .ShouldBeSameAs(expected, articles.Count(), pages.Count())
                    .ShouldHaveUpdateLink()
                    .ShouldHaveDeleteLink()
                    .ShouldHaveCreateArticlesLink()
                    .ShouldHaveCreatePageLink()
                    .ShouldHaveAddContentLink()
                    .ShouldHaveCorrectContents();
            }
        }


        [Test]
        public void ShouldHaveCorrectPaginationData()
        {
            _assert.ShouldHavePage(2)
                .ShouldHavePageSize(5)
                .ShouldHavePageCount(3)
                .ShouldHaveTotalCount(15);
        }
    }
}
