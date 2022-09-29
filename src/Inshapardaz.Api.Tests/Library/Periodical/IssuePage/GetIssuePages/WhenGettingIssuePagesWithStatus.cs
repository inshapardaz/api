using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.IssuePage.GetIssuePages
{
    [TestFixture(EditingStatus.All)]
    [TestFixture(EditingStatus.Typing)]
    [TestFixture(EditingStatus.Typed)]
    [TestFixture(EditingStatus.InReview)]
    [TestFixture(EditingStatus.Completed)]
    [TestFixture(EditingStatus.Available)]
    public class WhenGettingIssuePagesWithStatus : TestBase
    {
        private IssueDto _issue;
        private HttpResponseMessage _response;
        private PagingAssert<IssuePageView> _assert;
        private AccountDto _account;
        private readonly EditingStatus _status;

        public WhenGettingIssuePagesWithStatus(EditingStatus status)
            : base(Role.Reader)
        {
            _status = status;
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _account = AccountBuilder.Build();
            _issue = IssueBuilder.WithLibrary(LibraryId).WithPages(20)
                .WithStatus(_status, 12)
                .Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages?pageSize=10&pageNumber=1&status={_status.ToDescription()}");

            _assert = new PagingAssert<IssuePageView>(_response);
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
            if (_status == EditingStatus.All)
            {
                _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages");
            }
            else
            {
                _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages",
                    new KeyValuePair<string, string>("status", _status.ToDescription())
                );
            }
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            if (_status == EditingStatus.All)
            {
                _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages", 2, 10);
            }
            else
            {
                _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages", 2, 10,
                    new KeyValuePair<string, string>("status", _status.ToDescription())
                );
            }
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturExpectedBookPages()
        {
            var expectedItems = IssueBuilder.GetPages(_issue.Id)
                                            .Where(p => _status == EditingStatus.All || p.Status == _status)
                                            .OrderBy(p => p.SequenceNumber).Take(10);
            _assert.ShouldHaveTotalCount(20)
                   .ShouldHavePage(1)
                   .ShouldHavePageSize(10);

            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.SequenceNumber == item.SequenceNumber);
                actual.ShouldMatch(item)
                    .InLibrary(LibraryId)
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
