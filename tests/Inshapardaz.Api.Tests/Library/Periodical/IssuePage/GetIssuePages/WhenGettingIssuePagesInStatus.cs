using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.IssuePage.GetIssuePages
{
    [TestFixture(EditingStatus.InReview)]
    [TestFixture(EditingStatus.Completed)]
    [TestFixture(EditingStatus.Typed)]
    [TestFixture(EditingStatus.Typing)]
    [TestFixture(EditingStatus.Available)]
    public class WhenGettingIssuePagesInStatus : TestBase
    {
        private IssueDto _issue;
        private HttpResponseMessage _response;
        private PagingAssert<IssuePageView> _assert;
        private readonly EditingStatus _status;

        public WhenGettingIssuePagesInStatus(EditingStatus status)
            : base(Role.Reader)
        {
            _status = status;
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Build();
            _issue = IssueBuilder.WithLibrary(LibraryId).WithPages(20).WithStatus(_status, 15).Build();

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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages", 2, 10);
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturExpectedBookPages()
        {
            var expectedItems = IssueBuilder.GetPages(_issue.Id).Where(p => p.Status == _status).OrderBy(p => p.SequenceNumber).Take(10);

            _assert.ShouldHaveTotalCount(15)
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
