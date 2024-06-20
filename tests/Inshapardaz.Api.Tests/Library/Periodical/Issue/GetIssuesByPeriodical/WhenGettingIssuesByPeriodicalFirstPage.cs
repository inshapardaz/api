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
    public class WhenGettingIssuesByPeriodicalFirstPage
        : TestBase
    {
        private HttpResponseMessage _response;
        private IEnumerable<IssueDto> _issues;
        private PeriodicalDto _periodical;
        private PagingAssert<IssueView> _assert;

        public WhenGettingIssuesByPeriodicalFirstPage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _periodical = PeriodicalBuilder.WithLibrary(LibraryId).Build();
            _issues = IssueBuilder.WithLibrary(LibraryId).WithPeriodical(_periodical.Id).WithContent().Build(15);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_periodical.Id}/issues");
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/periodicals/{_periodical.Id}/issues");
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/periodicals/{_periodical.Id}/issues", 2);
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }
        
        [Test]
        public void ShouldHaveCreateLink()
        {
            _assert.ShouldHaveCreateLink($"/libraries/{LibraryId}/periodicals/{_periodical.Id}/issues");
        }

        [Test]
        public void ShouldHaveCorrectIssuessData()
        {
            var expectedItems = _issues.OrderBy(a => a.IssueDate).Take(10).ToArray();
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
            _assert.ShouldHavePage(1)
                .ShouldHavePageSize(10)
                .ShouldHavePageCount(2)
                .ShouldHaveTotalCount(15);
        }
    }
}
