using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.IssuePage.GetIssuePages
{
    [TestFixture]
    public class WhenGettingIssuePagesAssignedToMe : TestBase
    {
        private IssueDto _issue;
        private HttpResponseMessage _response;
        private PagingAssert<IssuePageView> _assert;
        private AccountDto _account;

        public WhenGettingIssuePagesAssignedToMe()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _account = AccountBuilder.Build();
            _issue = IssueBuilder.WithLibrary(LibraryId).WithPages(20)
                .AssignPagesToWriter(_account.Id, 7)
                .AssignPagesToWriter(AccountId, 11)
                .Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages?pageSize=10&pageNumber=1&assignmentFilter=assignedtome");

            _assert = Services.GetService<PagingAssert<IssuePageView>>().ForResponse(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages",
                new KeyValuePair<string, string>("assignmentFilter", "AssignedToMe"));
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages", 2, 10,
                new KeyValuePair<string, string>("assignmentFilter", "AssignedToMe"));
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturExpectedBookPages()
        {
            var expectedItems = IssueBuilder.GetPages(_issue.Id).Where(p => p.WriterAccountId == AccountId).OrderBy(p => p.SequenceNumber).Take(10);
            _assert.ShouldHaveTotalCount(11)
                   .ShouldHavePage(1)
                   .ShouldHavePageSize(10);

            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.SequenceNumber == item.SequenceNumber);
                Services.GetService<IssuePageAssert>().ForView(actual).ForLibrary(LibraryId)
                        .ShouldMatchWithoutText(item)
                        .ShouldHaveSelfLink()
                        .ShouldHavePeriodicalLink()
                        .ShouldHaveIssueLink()
                        .ShouldNotHaveImageLink()
                        .ShouldNotHaveUpdateLink()
                        .ShouldNotHaveDeleteLink()
                        .ShouldNotHaveImageUpdateLink()
                        .ShouldNotHaveImageDeleteLink();
            }
        }
    }
}
